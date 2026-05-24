namespace ScaleManagement.Domain.Enums;

/// <summary>
/// Controls the concurrency/continuity trade-off when allocating a number from a
/// <see cref="Entities.TenantSequence"/>.
/// </summary>
public enum NumberingMode
{
    /// <summary>
    /// High-concurrency allocation that commits independently of the caller's
    /// transaction. The sequence lock is released immediately, but a number is
    /// consumed even if the surrounding transaction later rolls back, so the
    /// resulting series may contain <b>gaps</b>. The right default for tickets.
    /// </summary>
    Fast = 0,

    /// <summary>
    /// Gap-free allocation that participates in the caller's transaction: the
    /// number is rolled back with the caller, guaranteeing a contiguous series.
    /// The sequence row stays locked until the caller commits, so allocations for
    /// the same key are serialised. Requires the caller to run inside
    /// <see cref="Repositories.IUnitOfWork.ExecuteInTransactionAsync(System.Func{System.Threading.CancellationToken, System.Threading.Tasks.Task}, System.Threading.CancellationToken)"/>;
    /// do not mix modes for one key.
    /// </summary>
    Gapless = 1
}
