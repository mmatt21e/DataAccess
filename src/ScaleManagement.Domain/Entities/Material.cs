using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// A commodity / product that is weighed (grain, aggregate, livestock feed,
/// liquid, etc.). Holds defaults that streamline data capture and the data
/// needed for volume conversions and billing.
/// </summary>
public class Material : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Tenant-unique material/product code.</summary>
    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Category { get; set; }

    /// <summary>Default unit weighments of this material are recorded in.</summary>
    public UnitOfMeasure DefaultUnit { get; set; } = UnitOfMeasure.Kilogram;

    /// <summary>Bulk density (mass per volume) used to derive volume for tank/silo scales. Null if not applicable.</summary>
    public decimal? Density { get; set; }

    /// <summary>Indicative unit price used by downstream billing (currency tracked separately by billing module).</summary>
    public decimal? UnitPrice { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
