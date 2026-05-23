using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Auditing;

/// <summary>
/// Caches <see cref="AuditConfiguration"/> rows and projects them into
/// <see cref="AuditPolicy"/> instances. Registered as a singleton; it reads
/// configuration through a short-lived scope so it never holds a captive
/// reference to the scoped <see cref="ScaleManagementDbContext"/>.
/// </summary>
public sealed class AuditPolicyProvider : IAuditPolicyProvider
{
    private const string CacheKey = "scale-management::audit-policies";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

    private readonly IMemoryCache _cache;
    private readonly IServiceScopeFactory _scopeFactory;

    public AuditPolicyProvider(IMemoryCache cache, IServiceScopeFactory scopeFactory)
    {
        _cache = cache;
        _scopeFactory = scopeFactory;
    }

    public AuditPolicy? GetPolicy(string entityName, Guid? tenantId)
    {
        var snapshot = GetSnapshot();

        if (tenantId.HasValue &&
            snapshot.TenantSpecific.TryGetValue((tenantId.Value, entityName), out var tenantPolicy))
        {
            return tenantPolicy;
        }

        return snapshot.Global.TryGetValue(entityName, out var globalPolicy) ? globalPolicy : null;
    }

    public void Invalidate() => _cache.Remove(CacheKey);

    private Snapshot GetSnapshot()
    {
        return _cache.GetOrCreate(CacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheTtl;
            return Load();
        })!;
    }

    private Snapshot Load()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ScaleManagementDbContext>();

        // AsNoTracking + IgnoreQueryFilters: configuration is not tenant filtered and
        // we want every row (global + all tenants) to build the lookup once.
        var configs = db.Set<AuditConfiguration>()
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(c => c.IsAuditEnabled)
            .ToList();

        var global = new Dictionary<string, AuditPolicy>(StringComparer.Ordinal);
        var tenantSpecific = new Dictionary<(Guid, string), AuditPolicy>();

        foreach (var c in configs)
        {
            var policy = new AuditPolicy(
                c.AuditOnInsert,
                c.AuditOnUpdate,
                c.AuditOnDelete,
                c.CaptureOldValues,
                c.CaptureNewValues,
                ParseExcluded(c.ExcludedProperties));

            if (c.TenantId.HasValue)
                tenantSpecific[(c.TenantId.Value, c.EntityName)] = policy;
            else
                global[c.EntityName] = policy;
        }

        return new Snapshot(global, tenantSpecific);
    }

    private static IReadOnlySet<string> ParseExcluded(string? csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
            return EmptySet;

        return csv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static readonly IReadOnlySet<string> EmptySet = new HashSet<string>(0);

    private sealed record Snapshot(
        IReadOnlyDictionary<string, AuditPolicy> Global,
        IReadOnlyDictionary<(Guid TenantId, string EntityName), AuditPolicy> TenantSpecific);
}
