using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// An external business party involved in transactions: a customer being billed,
/// a supplier delivering material, and/or a carrier hauling it. Drivers and
/// trucks may optionally be affiliated with a company.
/// </summary>
public class Company : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Tenant-unique company/account code.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Role(s) the company plays; a single company may be several at once.</summary>
    public CompanyType Type { get; set; } = CompanyType.Customer;

    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? StateOrProvince { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    /// <summary>Tax registration number used on tickets/invoices.</summary>
    public string? TaxId { get; set; }

    /// <summary>Account reference used by the billing module.</summary>
    public string? AccountNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    public ICollection<Truck> Trucks { get; set; } = new List<Truck>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
