using ScaleManagement.Domain.Common;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// Declares whether (and how) change-tracking auditing is performed for a given
/// entity type. Auditing is therefore <b>opt-in per entity and per tenant,
/// configured by data</b> — not hard-coded. The persistence layer consults the
/// matching configuration on every save and only writes <see cref="AuditLog"/>
/// rows when enabled for the relevant operation.
/// <para>
/// Resolution order for a given entity name: an exact tenant-specific row wins;
/// otherwise the global default row (<see cref="TenantId"/> == null) applies;
/// if neither exists, the entity is not audited.
/// </para>
/// </summary>
public class AuditConfiguration : BaseEntity, IAuditableEntity
{
    /// <summary>Null = global default applied to all tenants; otherwise the owning tenant.</summary>
    public Guid? TenantId { get; set; }

    /// <summary>CLR type name of the audited entity (e.g. "Transaction").</summary>
    public string EntityName { get; set; } = string.Empty;

    public bool IsAuditEnabled { get; set; } = true;

    public bool AuditOnInsert { get; set; } = true;
    public bool AuditOnUpdate { get; set; } = true;
    public bool AuditOnDelete { get; set; } = true;

    /// <summary>Capture the before-image (original values) for updates/deletes.</summary>
    public bool CaptureOldValues { get; set; } = true;

    /// <summary>Capture the after-image (current values) for inserts/updates.</summary>
    public bool CaptureNewValues { get; set; } = true;

    /// <summary>
    /// Comma-separated property names never written to the audit trail (secrets,
    /// PII, large blobs). Honoured when serialising old/new value snapshots.
    /// </summary>
    public string? ExcludedProperties { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? ModifiedAtUtc { get; set; }
    public string? ModifiedBy { get; set; }
}
