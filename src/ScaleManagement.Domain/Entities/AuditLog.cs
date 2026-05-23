using ScaleManagement.Domain.Common;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// An append-only record of a single change to an audited entity. Written
/// automatically by the persistence layer inside the same transaction as the
/// change it describes, so the trail can never diverge from the data. It is not
/// tenant-filtered at the row level (admins may need cross-tenant views) but
/// stores <see cref="TenantId"/> for scoping, and is itself never audited or
/// soft-deleted.
/// </summary>
public class AuditLog : BaseEntity
{
    public Guid? TenantId { get; set; }

    /// <summary>CLR type name of the changed entity.</summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>Primary key of the changed entity, stored as string to stay key-type agnostic.</summary>
    public string EntityId { get; set; } = string.Empty;

    public AuditAction Action { get; set; }

    /// <summary>JSON before-image (null for inserts or when capture disabled).</summary>
    public string? OldValues { get; set; }

    /// <summary>JSON after-image (null for deletes or when capture disabled).</summary>
    public string? NewValues { get; set; }

    /// <summary>JSON array of property names that changed (updates only).</summary>
    public string? ChangedColumns { get; set; }

    public string? UserId { get; set; }
    public string? UserName { get; set; }

    /// <summary>Correlation/trace id of the request that caused the change, if available.</summary>
    public string? CorrelationId { get; set; }

    public DateTimeOffset TimestampUtc { get; set; }
}
