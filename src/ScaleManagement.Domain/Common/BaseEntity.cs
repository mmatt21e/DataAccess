namespace ScaleManagement.Domain.Common;

/// <summary>
/// Base class for root entities that are not tenant scoped (for example the
/// <c>Tenant</c> itself and the append-only <c>AuditLog</c>).
/// </summary>
public abstract class BaseEntity : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

/// <summary>
/// Base class for the vast majority of entities: tenant scoped, provenance
/// stamped and soft deletable. Deriving from this single base is what wires an
/// entity into multi-tenancy, automatic auditing of who/when, and the soft
/// delete behaviour without any per-entity configuration.
/// </summary>
public abstract class TenantEntityBase : BaseEntity, ITenantScoped, IAuditableEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? ModifiedAtUtc { get; set; }
    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }
}
