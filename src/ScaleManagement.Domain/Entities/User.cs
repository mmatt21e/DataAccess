using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// An operator of the system within a tenant. Authentication is expected to be
/// handled by an external identity provider; only the subject reference and the
/// authorization role are persisted here (no plaintext secrets).
/// </summary>
public class User : TenantEntityBase
{
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Subject id from the external identity provider (OIDC "sub", AAD object id, …).</summary>
    public string? ExternalAuthId { get; set; }

    public UserRole Role { get; set; } = UserRole.Operator;

    public bool IsActive { get; set; } = true;
    public DateTimeOffset? LastLoginAtUtc { get; set; }

    /// <summary>Transactions operated by this user.</summary>
    public ICollection<Transaction> OperatedTransactions { get; set; } = new List<Transaction>();
}
