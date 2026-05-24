using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Repositories;

/// <summary>
/// Allocates the next value from a per-tenant numbering series in a
/// concurrency-safe way. Replaces the former racy "max sequence + 1" lookup:
/// the increment is performed atomically in the database so simultaneous callers
/// always receive distinct values.
/// </summary>
public interface INumberingRepository
{
    /// <summary>
    /// Atomically increments the series identified by <paramref name="key"/> for
    /// the current tenant and returns the allocated value, creating the series on
    /// first use.
    /// </summary>
    /// <param name="key">Series identifier (e.g. "Transaction", "Transaction:2026").</param>
    /// <param name="mode">
    /// <see cref="NumberingMode.Fast"/> (default) for high-concurrency allocation
    /// that may leave gaps on rollback, or <see cref="NumberingMode.Gapless"/> to
    /// allocate inside the caller's transaction for a contiguous series.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<long> NextAsync(string key, NumberingMode mode = NumberingMode.Fast, CancellationToken cancellationToken = default);
}
