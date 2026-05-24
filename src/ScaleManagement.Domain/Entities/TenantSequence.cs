using ScaleManagement.Domain.Common;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// A per-tenant, per-key monotonic counter backing human-readable number
/// allocation (transaction/ticket numbers, etc.). The next value is handed out
/// by an atomic database operation rather than read-modify-write, so concurrent
/// callers never receive the same number without relying on a row-version token.
/// <para>
/// <see cref="Key"/> lets the business layer choose the numbering policy
/// (one series per tenant, per year, or per scale type) without any schema
/// change — e.g. "Transaction", "Transaction:2026" or "Transaction:TruckScale".
/// </para>
/// </summary>
public class TenantSequence : BaseEntity, ITenantScoped
{
    public Guid TenantId { get; set; }

    /// <summary>Identifies the series within the tenant. Unique together with <see cref="TenantId"/>.</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>The value that will be returned by the next allocation.</summary>
    public long NextValue { get; set; } = 1;
}
