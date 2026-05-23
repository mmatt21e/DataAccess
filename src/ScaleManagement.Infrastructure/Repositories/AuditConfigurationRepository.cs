using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

public sealed class AuditConfigurationRepository : RepositoryBase<AuditConfiguration>, IAuditConfigurationRepository
{
    public AuditConfigurationRepository(ScaleManagementDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AuditConfiguration>> ListAllAsync(CancellationToken cancellationToken = default)
        => await Set.IgnoreQueryFilters().AsNoTracking().ToListAsync(cancellationToken);

    public async Task<AuditConfiguration?> GetEffectiveAsync(string entityName, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        var candidates = await Set.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(c => c.EntityName == entityName && (c.TenantId == null || c.TenantId == tenantId))
            .ToListAsync(cancellationToken);

        // Prefer a tenant-specific row over the global default.
        return candidates.FirstOrDefault(c => c.TenantId == tenantId)
            ?? candidates.FirstOrDefault(c => c.TenantId == null);
    }
}
