namespace ScaleManagement.Domain.Common;

/// <summary>
/// Lightweight "who/when" provenance stamped automatically by the persistence
/// layer. This is distinct from the full change-tracking audit trail
/// (<c>AuditLog</c>): these columns live on the row itself, the audit trail is
/// a separate append-only history.
/// </summary>
public interface IAuditableEntity
{
    DateTimeOffset CreatedAtUtc { get; set; }
    string? CreatedBy { get; set; }
    DateTimeOffset? ModifiedAtUtc { get; set; }
    string? ModifiedBy { get; set; }
}
