using ScaleManagement.Domain.Enums;
using ScaleManagement.Domain.Repositories;

namespace ScaleManagement.Domain.Reporting;

/// <summary>
/// Dedicated read/query surface for transaction reporting. Implemented with
/// efficient, no-tracking projections in the persistence layer and consumed by
/// the Reporting library. Kept separate from <see cref="ITransactionRepository"/>
/// so the write model and the report-shaped read model evolve independently.
/// All queries run within the active tenant scope.
/// </summary>
public interface ITransactionReportQueries
{
    /// <summary>Server-side paged report search returning display-ready summary rows.</summary>
    Task<PagedResult<TransactionSummaryRow>> SearchAsync(TransactionReportQuery query, CancellationToken cancellationToken = default);

    /// <summary>Net-weight totals grouped by material across a (tenant-local) date range.</summary>
    Task<IReadOnlyList<MaterialTotalsRow>> MaterialTotalsAsync(DateOnly fromInclusive, DateOnly toInclusive, CancellationToken cancellationToken = default);

    /// <summary>Per-day completed-transaction throughput for a single scale.</summary>
    Task<IReadOnlyList<DailyThroughputRow>> DailyThroughputAsync(Guid scaleId, DateOnly fromInclusive, DateOnly toInclusive, CancellationToken cancellationToken = default);
}

/// <summary>Filter/sort/paging options for <see cref="ITransactionReportQueries.SearchAsync"/>.</summary>
public sealed class TransactionReportQuery
{
    public Guid? ScaleId { get; init; }
    public ScaleType? ScaleType { get; init; }
    public Guid? CompanyId { get; init; }
    public Guid? MaterialId { get; init; }
    public TransactionStatus? Status { get; init; }
    public TransactionDirection? Direction { get; init; }
    public DateTimeOffset? FromUtc { get; init; }
    public DateTimeOffset? ToUtc { get; init; }
    public string? TextSearch { get; init; }

    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
    public bool Descending { get; init; } = true;
}
