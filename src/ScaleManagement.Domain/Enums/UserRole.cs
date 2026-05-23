namespace ScaleManagement.Domain.Enums;

/// <summary>
/// Coarse-grained authorization role for an operator. Fine-grained permissions
/// would typically layer on top of this; it is kept simple here as the data
/// access layer only needs to persist the assignment.
/// </summary>
public enum UserRole
{
    Operator = 1,
    Supervisor = 2,
    Administrator = 3,
    TenantOwner = 4,
    ReadOnly = 5
}
