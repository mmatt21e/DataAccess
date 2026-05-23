using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ScaleManagement.Domain.Abstractions;
using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Single coordinated <see cref="SaveChangesInterceptor"/> that runs on every
/// commit and performs four ordered concerns. They are deliberately combined in
/// one interceptor because they are order-sensitive (provenance must be stamped
/// before the audit snapshot is taken, and the audit snapshot must be taken
/// before soft-deletes are rewritten from Deleted to Modified):
/// <list type="number">
///   <item>stamp <see cref="ITenantScoped.TenantId"/> on newly added rows;</item>
///   <item>stamp created/modified provenance on <see cref="IAuditableEntity"/>;</item>
///   <item>write <see cref="AuditLog"/> entries for entities whose
///         <c>AuditConfiguration</c> opts in, honouring per-entity policy; and</item>
///   <item>convert hard deletes of <see cref="ISoftDeletable"/> entities into
///         flag updates.</item>
/// </list>
/// All of this happens inside the same transaction as the change itself.
/// </summary>
public sealed class AuditingSaveChangesInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    private readonly ITenantContext _tenantContext;
    private readonly IClock _clock;
    private readonly Auditing.IAuditPolicyProvider _auditPolicies;

    public AuditingSaveChangesInterceptor(
        ITenantContext tenantContext,
        IClock clock,
        Auditing.IAuditPolicyProvider auditPolicies)
    {
        _tenantContext = tenantContext;
        _clock = clock;
        _auditPolicies = auditPolicies;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext context)
    {
        var now = _clock.UtcNow;

        // Snapshot the entries up front: we add AuditLog rows below and must not
        // iterate over them here.
        var entries = context.ChangeTracker.Entries().ToList();

        var auditLogs = new List<AuditLog>();

        foreach (var entry in entries)
        {
            // Never stamp/audit the trail itself.
            if (entry.Entity is AuditLog)
                continue;

            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            // 1 + 2: stamp tenant + provenance using the original state.
            StampTenant(entry);
            StampProvenance(entry, now);

            // 3: capture audit snapshot while the state still reflects the real action.
            var log = TryBuildAuditLog(entry, now);
            if (log is not null)
                auditLogs.Add(log);

            // 4: convert soft deletes after the snapshot has been taken.
            ConvertSoftDelete(entry, now);
        }

        if (auditLogs.Count > 0)
            context.Set<AuditLog>().AddRange(auditLogs);
    }

    private void StampTenant(EntityEntry entry)
    {
        if (entry.State == EntityState.Added &&
            entry.Entity is ITenantScoped scoped &&
            scoped.TenantId == Guid.Empty &&
            _tenantContext.TenantId.HasValue)
        {
            scoped.TenantId = _tenantContext.TenantId.Value;
        }
    }

    private void StampProvenance(EntityEntry entry, DateTimeOffset now)
    {
        if (entry.Entity is not IAuditableEntity auditable)
            return;

        switch (entry.State)
        {
            case EntityState.Added:
                auditable.CreatedAtUtc = now;
                auditable.CreatedBy = _tenantContext.UserId;
                break;
            case EntityState.Modified:
                auditable.ModifiedAtUtc = now;
                auditable.ModifiedBy = _tenantContext.UserId;
                break;
        }
    }

    private void ConvertSoftDelete(EntityEntry entry, DateTimeOffset now)
    {
        if (entry.State != EntityState.Deleted || entry.Entity is not ISoftDeletable deletable)
            return;

        entry.State = EntityState.Modified;
        deletable.IsDeleted = true;
        deletable.DeletedAtUtc = now;
        deletable.DeletedBy = _tenantContext.UserId;

        if (entry.Entity is IAuditableEntity auditable)
        {
            auditable.ModifiedAtUtc = now;
            auditable.ModifiedBy = _tenantContext.UserId;
        }
    }

    private AuditLog? TryBuildAuditLog(EntityEntry entry, DateTimeOffset now)
    {
        var action = entry.State switch
        {
            EntityState.Added => AuditAction.Insert,
            EntityState.Modified => AuditAction.Update,
            EntityState.Deleted => AuditAction.Delete,
            _ => (AuditAction?)null
        };

        if (action is null)
            return null;

        var entityName = entry.Metadata.ClrType.Name;

        var tenantId = entry.Entity is ITenantScoped scoped && scoped.TenantId != Guid.Empty
            ? scoped.TenantId
            : _tenantContext.TenantId;

        var policy = _auditPolicies.GetPolicy(entityName, tenantId);
        if (policy is null || !policy.ShouldAudit(action.Value))
            return null;

        string? oldValues = null;
        string? newValues = null;
        string? changedColumns = null;

        if (action == AuditAction.Insert)
        {
            if (policy.CaptureNewValues)
                newValues = Serialize(entry, policy, current: true);
        }
        else if (action == AuditAction.Delete)
        {
            if (policy.CaptureOldValues)
                oldValues = Serialize(entry, policy, current: false);
        }
        else // Update
        {
            if (policy.CaptureOldValues)
                oldValues = Serialize(entry, policy, current: false);
            if (policy.CaptureNewValues)
                newValues = Serialize(entry, policy, current: true);
            changedColumns = SerializeChangedColumns(entry, policy);
        }

        return new AuditLog
        {
            TenantId = tenantId,
            EntityName = entityName,
            EntityId = ResolveKey(entry),
            Action = action.Value,
            OldValues = oldValues,
            NewValues = newValues,
            ChangedColumns = changedColumns,
            UserId = _tenantContext.UserId,
            UserName = _tenantContext.UserName,
            CorrelationId = _tenantContext.CorrelationId,
            TimestampUtc = now
        };
    }

    private static string ResolveKey(EntityEntry entry)
    {
        if (entry.Entity is IEntity e)
            return e.Id.ToString();

        var key = entry.Metadata.FindPrimaryKey();
        if (key is null)
            return string.Empty;

        var values = key.Properties.Select(p => entry.Property(p.Name).CurrentValue?.ToString() ?? string.Empty);
        return string.Join("|", values);
    }

    private static string Serialize(EntityEntry entry, Auditing.AuditPolicy policy, bool current)
    {
        var values = new Dictionary<string, object?>(StringComparer.Ordinal);

        foreach (var prop in entry.Properties)
        {
            var name = prop.Metadata.Name;
            if (policy.ExcludedProperties.Contains(name))
                continue;

            values[name] = current ? prop.CurrentValue : prop.OriginalValue;
        }

        return JsonSerializer.Serialize(values, JsonOptions);
    }

    private static string SerializeChangedColumns(EntityEntry entry, Auditing.AuditPolicy policy)
    {
        var changed = entry.Properties
            .Where(p => p.IsModified && !policy.ExcludedProperties.Contains(p.Metadata.Name))
            .Select(p => p.Metadata.Name)
            .ToList();

        return JsonSerializer.Serialize(changed, JsonOptions);
    }
}
