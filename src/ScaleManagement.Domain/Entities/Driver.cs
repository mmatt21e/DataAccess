using ScaleManagement.Domain.Common;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// A person driving a vehicle across the scale. Optionally affiliated with a
/// <see cref="Company"/> (carrier).
/// </summary>
public class Driver : TenantEntityBase
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    /// <summary>Driving licence / permit number.</summary>
    public string? LicenseNumber { get; set; }

    public string? Phone { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
