namespace ScaleManagement.Infrastructure.Persistence;

/// <summary>
/// Controls how <see cref="IDatabaseInitializer"/> behaves when the data access
/// layer is initialized. Supplied through
/// <c>AddScaleManagementDataAccess(..., configureInitialization)</c>.
/// </summary>
public sealed class DatabaseInitializationOptions
{
    /// <summary>
    /// When true (default), the database is created automatically if it does not
    /// already exist. The connection string must point at a server the caller has
    /// permission to create databases on. When false the initializer only reports
    /// whether the database exists and never creates it.
    /// </summary>
    public bool CreateIfNotExists { get; set; } = true;

    /// <summary>
    /// When true (default), creation and updates go through EF Core migrations
    /// (<c>Database.MigrateAsync</c>). If no migrations exist in the assembly yet,
    /// the initializer transparently falls back to <c>EnsureCreated</c> so a usable
    /// schema is still produced. When false, the schema is always created directly
    /// from the model via <c>EnsureCreated</c> (suitable for tests/prototypes, but
    /// not upgradable with migrations afterwards).
    /// </summary>
    public bool UseMigrations { get; set; } = true;

    /// <summary>
    /// When true (default) and <see cref="UseMigrations"/> is enabled, any pending
    /// migrations are applied to an already-existing database during initialization.
    /// </summary>
    public bool ApplyPendingMigrations { get; set; } = true;
}
