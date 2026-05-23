namespace ScaleManagement.Domain.Enums;

/// <summary>
/// Role a company plays in transactions. Marked <see cref="FlagsAttribute"/> so a
/// single company can be, for example, both a customer and a carrier.
/// </summary>
[Flags]
public enum CompanyType
{
    None = 0,
    Customer = 1,
    Supplier = 2,
    Carrier = 4,
    Broker = 8
}
