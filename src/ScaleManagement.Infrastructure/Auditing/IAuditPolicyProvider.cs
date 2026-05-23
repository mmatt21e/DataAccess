namespace ScaleManagement.Infrastructure.Auditing;

/// <summary>
/// Supplies the effective <see cref="AuditPolicy"/> for an entity type, applying
/// the tenant-specific-over-global resolution rule. Results are cached so the
/// hot save path does not hit the database on every commit; <see cref="Invalidate"/>
/// is called after audit configuration changes.
/// </summary>
public interface IAuditPolicyProvider
{
    /// <summary>Returns the policy for an entity, or null when auditing is not enabled for it.</summary>
    AuditPolicy? GetPolicy(string entityName, Guid? tenantId);

    /// <summary>Drops the cache so the next lookup reloads configuration from the store.</summary>
    void Invalidate();
}
