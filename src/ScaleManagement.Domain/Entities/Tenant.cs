using ScaleManagement.Domain.Common;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// The top-level isolation boundary: a single customer of the SaaS platform
/// (a site operator, weighbridge company, plant, etc.). Every tenant-scoped
/// row carries this tenant's id and is invisible to other tenants via a global
/// query filter. The tenant is itself a root (not tenant scoped).
/// </summary>
public class Tenant : BaseEntity, IAuditableEntity
{
    /// <summary>Human readable name of the tenant organisation.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Stable unique slug/code used in URLs, integrations and licensing.</summary>
    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    /// <summary>IANA time zone id used to localise weighment timestamps for this tenant.</summary>
    public string TimeZoneId { get; set; } = "UTC";

    /// <summary>Default measurement system applied to new scales/materials for this tenant.</summary>
    public string DefaultCulture { get; set; } = "en-US";

    public DateTimeOffset CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? ModifiedAtUtc { get; set; }
    public string? ModifiedBy { get; set; }

    /// <summary>Scale modules this tenant has enabled. Drives feature availability purely by data.</summary>
    public ICollection<TenantModule> Modules { get; set; } = new List<TenantModule>();
}
