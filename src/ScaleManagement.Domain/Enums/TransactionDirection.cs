namespace ScaleManagement.Domain.Enums;

/// <summary>Direction of goods relative to the site for a weighment.</summary>
public enum TransactionDirection
{
    Unspecified = 0,
    Inbound = 1,
    Outbound = 2,
    Internal = 3,
    Transfer = 4
}
