using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace ScaleManagement.Infrastructure.Persistence;

/// <summary>
/// Default <see cref="IDatabaseInitializer"/>. Uses the relational provider's
/// database creator to test for existence, then creates the database via
/// migrations or <c>EnsureCreated</c> according to
/// <see cref="DatabaseInitializationOptions"/>. Runs with whatever tenant context
/// is active (typically unscoped at startup), which is correct for DDL.
/// </summary>
public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly ScaleManagementDbContext _context;
    private readonly DatabaseInitializationOptions _options;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        ScaleManagementDbContext context,
        DatabaseInitializationOptions options,
        ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _options = options;
        _logger = logger;
    }

    public Task<bool> DatabaseExistsAsync(CancellationToken cancellationToken = default)
    {
        var creator = _context.Database.GetService<IRelationalDatabaseCreator>();
        return creator.ExistsAsync(cancellationToken);
    }

    public async Task<DatabaseInitializationResult> InitializeAsync(CancellationToken cancellationToken = default)
    {
        var creator = _context.Database.GetService<IRelationalDatabaseCreator>();
        var existed = await creator.ExistsAsync(cancellationToken);

        if (!existed)
            return await CreateAsync(cancellationToken);

        _logger.LogDebug("Scale management database already exists.");
        var applied = await ApplyPendingAsync(cancellationToken);
        return new DatabaseInitializationResult(true, false, applied);
    }

    private async Task<DatabaseInitializationResult> CreateAsync(CancellationToken cancellationToken)
    {
        if (!_options.CreateIfNotExists)
        {
            _logger.LogWarning(
                "Scale management database does not exist and CreateIfNotExists is disabled; skipping creation.");
            return new DatabaseInitializationResult(false, false, Array.Empty<string>());
        }

        var migrations = _context.Database.GetMigrations().ToList();

        if (_options.UseMigrations && migrations.Count > 0)
        {
            _logger.LogInformation(
                "Scale management database not found; creating via {Count} migration(s).", migrations.Count);
            await _context.Database.MigrateAsync(cancellationToken);
            return new DatabaseInitializationResult(false, true, migrations);
        }

        if (_options.UseMigrations)
        {
            _logger.LogWarning(
                "UseMigrations is enabled but no migrations were found in the assembly; " +
                "falling back to EnsureCreated. Generate migrations to enable schema upgrades.");
        }
        else
        {
            _logger.LogInformation("Scale management database not found; creating schema via EnsureCreated.");
        }

        await _context.Database.EnsureCreatedAsync(cancellationToken);
        return new DatabaseInitializationResult(false, true, Array.Empty<string>());
    }

    private async Task<IReadOnlyList<string>> ApplyPendingAsync(CancellationToken cancellationToken)
    {
        if (!_options.UseMigrations || !_options.ApplyPendingMigrations)
            return Array.Empty<string>();

        if (!_context.Database.GetMigrations().Any())
            return Array.Empty<string>();

        try
        {
            var pending = (await _context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
            if (pending.Count == 0)
                return Array.Empty<string>();

            _logger.LogInformation("Applying {Count} pending migration(s).", pending.Count);
            await _context.Database.MigrateAsync(cancellationToken);
            return pending;
        }
        catch (Exception ex)
        {
            // A database created with EnsureCreated has no migrations-history table, so
            // pending-migration checks cannot run against it. Surface a clear warning
            // rather than failing initialization.
            _logger.LogWarning(ex,
                "Could not apply pending migrations. If the database was created with EnsureCreated, " +
                "migrations and EnsureCreated cannot be mixed.");
            return Array.Empty<string>();
        }
    }
}
