using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScaleManagement.Domain.Abstractions;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Auditing;
using ScaleManagement.Infrastructure.Persistence;
using ScaleManagement.Infrastructure.Persistence.Interceptors;
using ScaleManagement.Infrastructure.Repositories;
using ScaleManagement.Infrastructure.Tenancy;

namespace ScaleManagement.Infrastructure;

/// <summary>
/// Composition root for the data access layer. A host registers the whole layer
/// with a single call and supplies only the provider/connection details, e.g.:
/// <code>
/// services.AddScaleManagementDataAccess(options =>
///     options.UseSqlServer(connectionString));
/// </code>
/// The host is expected to populate the scoped <see cref="AmbientTenantContext"/>
/// per request/operation (or register its own <see cref="ITenantContext"/>).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddScaleManagementDataAccess(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDbContext)
    {
        ArgumentNullException.ThrowIfNull(configureDbContext);

        services.AddMemoryCache();

        // Ambient context + clock. TryAdd lets a host override with its own
        // (e.g. an HttpContext-backed tenant context) before calling this.
        services.TryAddScoped<AmbientTenantContext>();
        services.TryAddScoped<ITenantContext>(sp => sp.GetRequiredService<AmbientTenantContext>());
        services.TryAddSingleton<IClock, SystemClock>();

        // Auditing policy cache + the coordinated save interceptor.
        services.TryAddSingleton<IAuditPolicyProvider, AuditPolicyProvider>();
        services.AddScoped<AuditingSaveChangesInterceptor>();

        services.AddDbContext<ScaleManagementDbContext>((sp, options) =>
        {
            configureDbContext(options);
            options.AddInterceptors(sp.GetRequiredService<AuditingSaveChangesInterceptor>());
        });

        AddRepositories(services);

        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));

        services.AddScoped<IScaleRepository, ScaleRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IMaterialRepository, MaterialRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<ITruckRepository, TruckRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IQueueRepository, QueueRepository>();
        services.AddScoped<IAuditConfigurationRepository, AuditConfigurationRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
