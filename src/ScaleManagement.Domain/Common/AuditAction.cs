namespace ScaleManagement.Domain.Common;

/// <summary>
/// The persistence operation captured by an audit trail entry.
/// </summary>
public enum AuditAction
{
    Insert = 1,
    Update = 2,
    Delete = 3
}
