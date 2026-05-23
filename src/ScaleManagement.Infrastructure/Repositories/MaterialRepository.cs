using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

public sealed class MaterialRepository : RepositoryBase<Material>, IMaterialRepository
{
    public MaterialRepository(ScaleManagementDbContext context) : base(context) { }

    public Task<Material?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(m => m.Code == code, cancellationToken);

    public async Task<IReadOnlyList<Material>> ListActiveAsync(CancellationToken cancellationToken = default)
        => await Set.Where(m => m.IsActive).OrderBy(m => m.Name).ToListAsync(cancellationToken);
}
