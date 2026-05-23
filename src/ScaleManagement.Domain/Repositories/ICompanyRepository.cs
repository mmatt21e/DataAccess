using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Repositories;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Company>> ListByTypeAsync(CompanyType type, CancellationToken cancellationToken = default);
}
