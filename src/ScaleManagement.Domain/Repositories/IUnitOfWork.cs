using ScaleManagement.Domain.Common;

namespace ScaleManagement.Domain.Repositories;

/// <summary>
/// Coordinates the work of multiple repositories against a single shared
/// change-tracking context and commits it atomically. Inject this (rather than
/// individual repositories) when an operation must update several aggregates in
/// one transaction — e.g. completing a queue entry and creating its transaction.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IScaleRepository Scales { get; }
    ITransactionRepository Transactions { get; }
    IMaterialRepository Materials { get; }
    ICompanyRepository Companies { get; }
    IDriverRepository Drivers { get; }
    ITruckRepository Trucks { get; }
    IUserRepository Users { get; }
    IQueueRepository Queues { get; }
    IAuditConfigurationRepository AuditConfigurations { get; }
    IAuditLogRepository AuditLogs { get; }

    /// <summary>
    /// Resolves a generic repository for any entity that lacks a bespoke
    /// interface (e.g. <c>TenantModule</c>, <c>TransactionMeasurement</c>,
    /// <c>QueueEntry</c>), keeping the common CRUD surface available everywhere.
    /// </summary>
    IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;

    /// <summary>Persists all tracked changes; provenance, tenant stamping and auditing run automatically.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs <paramref name="operation"/> inside a database transaction with the
    /// provider's execution strategy (resilient to transient failures),
    /// committing on success and rolling back on exception.
    /// </summary>
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);
}
