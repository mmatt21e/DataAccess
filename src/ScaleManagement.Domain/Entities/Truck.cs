using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// A vehicle weighed on truck scales. Supports a stored/permanent tare so that
/// single-pass weighing can compute net weight without a second weighment.
/// </summary>
public class Truck : TenantEntityBase
{
    /// <summary>Registration plate / fleet number (tenant-unique).</summary>
    public string LicensePlate { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Make { get; set; }
    public int? AxleCount { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    /// <summary>Stored tare weight used for single-pass (stored-tare) weighing.</summary>
    public decimal? StoredTareWeight { get; set; }

    public UnitOfMeasure StoredTareUnit { get; set; } = UnitOfMeasure.Kilogram;

    /// <summary>Expiry of the stored tare; past this the operator must re-tare.</summary>
    public DateTimeOffset? TareValidUntilUtc { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
