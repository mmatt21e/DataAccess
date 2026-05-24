using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
    /// <param name="configureDbContext">Provider/connection configuration (e.g. <c>options.UseSqlServer(...)</c>).</param>
    /// <param name="configureInitialization">
    /// Optional database-initialization settings. When supplied (or left at its
    /// defaults), an <see cref="IDatabaseInitializer"/> is registered that can
    /// check for the database and create it. Call
    /// <see cref="InitializeScaleManagementDatabaseAsync"/> at startup to run it.
    /// </param>
    public static IServiceCollection AddScaleManagementDataAccess(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDbContext,
        Action<DatabaseInitializationOptions>? configureInitialization = null)
    {
        ArgumentNullException.ThrowIfNull(configureDbContext);

        services.AddMemoryCache();

        var initializationOptions = new DatabaseInitializationOptions();
        configureInitialization?.Invoke(initializationOptions);
        services.TryAddSingleton(initializationOptions);
        services.AddScoped<IDatabaseInitializer>(sp => new DatabaseInitializer(
            sp.GetRequiredService<ScaleManagementDbContext>(),
            sp.GetRequiredService<DatabaseInitializationOptions>(),
            sp.GetService<ILogger<DatabaseInitializer>>() ?? NullLogger<DatabaseInitializer>.Instance));

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

    /// <summary>
    /// Runs database initialization (existence check + optional create/migrate) in
    /// a fresh scope. Call this once at host startup, for example:
    /// <code>
    /// var result = await app.Services.InitializeScaleManagementDatabaseAsync();
    /// </code>
    /// </summary>
    public static async Task<DatabaseInitializationResult> InitializeScaleManagementDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        return await initializer.InitializeAsync(cancellationToken);
    }
}
