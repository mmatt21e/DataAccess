using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<Transaction?> GetByNumberAsync(string transactionNumber, CancellationToken cancellationToken = default);

    /// <summary>Loads a transaction together with its full set of measurements.</summary>
    Task<Transaction?> GetWithMeasurementsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Transaction>> ListByScaleAsync(Guid scaleId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Transaction>> ListOpenAsync(CancellationToken cancellationToken = default);

    /// <summary>Open two-pass transaction for a given truck (used to find the matching first weighment).</summary>
    Task<Transaction?> GetOpenForTruckAsync(Guid truckId, CancellationToken cancellationToken = default);

    /// <summary>Server-side paged search over transactions with the common scale-management filters.</summary>
    Task<PagedResult<Transaction>> SearchAsync(TransactionQuery query, CancellationToken cancellationToken = default);

    /// <summary>Highest existing transaction number sequence for the tenant, used to allocate the next ticket.</summary>
    Task<long> GetMaxSequenceAsync(CancellationToken cancellationToken = default);
}

/// <summary>Filter/sort/paging options for <see cref="ITransactionRepository.SearchAsync"/>.</summary>
public sealed class TransactionQuery
{
    public Guid? ScaleId { get; init; }
    public ScaleType? ScaleType { get; init; }
    public Guid? CompanyId { get; init; }
    public Guid? MaterialId { get; init; }
    public Guid? TruckId { get; init; }
    public Guid? DriverId { get; init; }
    public TransactionStatus? Status { get; init; }
    public TransactionDirection? Direction { get; init; }
    public DateTimeOffset? FromUtc { get; init; }
    public DateTimeOffset? ToUtc { get; init; }
    public string? TextSearch { get; init; }

    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
    public bool Descending { get; init; } = true;
}
