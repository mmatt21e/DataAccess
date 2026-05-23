using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Domain.Repositories;

public interface IMaterialRepository : IRepository<Material>
{
    Task<Material?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Material>> ListActiveAsync(CancellationToken cancellationToken = default);
}
