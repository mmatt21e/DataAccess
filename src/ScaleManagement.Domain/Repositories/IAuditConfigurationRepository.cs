using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Domain.Repositories;

public interface IAuditConfigurationRepository : IRepository<AuditConfiguration>
{
    /// <summary>All configurations (global + tenant-specific) for refreshing the audit policy cache.</summary>
    Task<IReadOnlyList<AuditConfiguration>> ListAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Resolves the effective configuration for an entity, preferring a tenant-specific row over the global default.</summary>
    Task<AuditConfiguration?> GetEffectiveAsync(string entityName, Guid? tenantId, CancellationToken cancellationToken = default);
}
