namespace ScaleManagement.Domain.Enums;

/// <summary>
/// Lifecycle state of a weighing transaction. An "Open" transaction is common
/// for two-pass truck weighing where the vehicle has weighed in but not yet out.
/// </summary>
public enum TransactionStatus
{
    Draft = 0,

    /// <summary>Awaiting a second weight or additional drafts.</summary>
    Open = 1,

    /// <summary>Captured but pending review/approval.</summary>
    Pending = 2,

    Completed = 3,

    /// <summary>Reversed after completion (kept for audit, excluded from totals).</summary>
    Voided = 4,

    Cancelled = 5
}
