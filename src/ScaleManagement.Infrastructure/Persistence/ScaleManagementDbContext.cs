using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Abstractions;
using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence;

/// <summary>
/// The Entity Framework Core unit of work for the scale management platform.
/// <para>
/// Two cross-cutting behaviours are wired in here so they apply uniformly and
/// cannot be forgotten per query/entity:
/// </para>
/// <list type="bullet">
///   <item><b>Multi-tenancy</b> — a global query filter restricts every
///   <see cref="ITenantScoped"/> entity to the current tenant, giving row-level
///   isolation in a shared schema. When no tenant is set (migrations, admin
///   tooling) the filter is bypassed.</item>
///   <item><b>Soft delete</b> — a global query filter hides
///   <see cref="ISoftDeletable"/> rows flagged as deleted.</item>
/// </list>
/// Provenance stamping and audit-trail writing are handled by the registered
/// <see cref="Interceptors.AuditingSaveChangesInterceptor"/>.
/// </summary>
public class ScaleManagementDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public ScaleManagementDbContext(
        DbContextOptions<ScaleManagementDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Current tenant as seen by the global query filter. Public so the filter's
    /// expression (built via reflection) can bind to it; EF re-evaluates this on
    /// every query, so one cached model serves all tenants correctly.
    /// </summary>
    public Guid? CurrentTenantId => _tenantContext.TenantId;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantModule> TenantModules => Set<TenantModule>();
    public DbSet<Scale> Scales => Set<Scale>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionMeasurement> TransactionMeasurements => Set<TransactionMeasurement>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Truck> Trucks => Set<Truck>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Queue> Queues => Set<Queue>();
    public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();
    public DbSet<AuditConfiguration> AuditConfigurations => Set<AuditConfiguration>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Pick up every IEntityTypeConfiguration<T> in this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScaleManagementDbContext).Assembly);

        ApplyGlobalQueryFilters(modelBuilder);
    }

    /// <summary>
    /// Builds the combined tenant + soft-delete query filter for each entity that
    /// implements the relevant marker interface. The tenant id is read from the
    /// context field on every query (EF parameterises the member access), so a
    /// single cached model serves all tenants correctly.
    /// </summary>
    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            var isTenantScoped = typeof(ITenantScoped).IsAssignableFrom(clrType);
            var isSoftDeletable = typeof(ISoftDeletable).IsAssignableFrom(clrType);

            if (!isTenantScoped && !isSoftDeletable)
                continue;

            var parameter = Expression.Parameter(clrType, "e");
            Expression? body = null;

            if (isTenantScoped)
            {
                // !CurrentTenantId.HasValue || e.TenantId == CurrentTenantId.Value
                var contextConstant = Expression.Constant(this);
                var currentTenant = Expression.Property(contextConstant, nameof(CurrentTenantId)); // Guid?
                var hasValue = Expression.Property(currentTenant, nameof(Nullable<Guid>.HasValue));
                var tenantValue = Expression.Property(currentTenant, nameof(Nullable<Guid>.Value));
                var entityTenant = Expression.Property(parameter, nameof(ITenantScoped.TenantId));

                var tenantMatch = Expression.OrElse(
                    Expression.Not(hasValue),
                    Expression.Equal(entityTenant, tenantValue));

                body = tenantMatch;
            }

            if (isSoftDeletable)
            {
                var isDeleted = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var notDeleted = Expression.Not(isDeleted);
                body = body is null ? notDeleted : Expression.AndAlso(body, notDeleted);
            }

            var lambda = Expression.Lambda(body!, parameter);
            modelBuilder.Entity(clrType).HasQueryFilter(lambda);
        }
    }
}
