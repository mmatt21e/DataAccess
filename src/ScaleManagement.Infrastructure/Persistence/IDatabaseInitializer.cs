namespace ScaleManagement.Infrastructure.Persistence;

/// <summary>
/// Checks for the existence of the scale management database and, when
/// configured to, creates and/or migrates it. Intended to be invoked once at
/// host startup, before serving requests.
/// </summary>
public interface IDatabaseInitializer
{
    /// <summary>Returns true if the configured database currently exists on the server.</summary>
    Task<bool> DatabaseExistsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures the database is present (creating/migrating per
    /// <see cref="DatabaseInitializationOptions"/>) and reports what was done.
    /// </summary>
    Task<DatabaseInitializationResult> InitializeAsync(CancellationToken cancellationToken = default);
}

/// <summary>Outcome of a call to <see cref="IDatabaseInitializer.InitializeAsync"/>.</summary>
/// <param name="DatabaseAlreadyExisted">True if the database existed before initialization.</param>
/// <param name="DatabaseCreated">True if this call created the database.</param>
/// <param name="AppliedMigrations">Migrations applied during this call (empty when none / not using migrations).</param>
public sealed record DatabaseInitializationResult(
    bool DatabaseAlreadyExisted,
    bool DatabaseCreated,
    IReadOnlyList<string> AppliedMigrations);
