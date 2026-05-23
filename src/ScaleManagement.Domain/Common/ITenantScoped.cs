namespace ScaleManagement.Domain.Common;

/// <summary>
/// Implemented by every entity that belongs to a single tenant. The presence of
/// this interface drives the automatic tenant global query filter and the
/// automatic stamping of <see cref="TenantId"/> on insert, giving row-level
/// isolation in a shared-database / shared-schema multi-tenant model.
/// </summary>
public interface ITenantScoped
{
    Guid TenantId { get; set; }
}
