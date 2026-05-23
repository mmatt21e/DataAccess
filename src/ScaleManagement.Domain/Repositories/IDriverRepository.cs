using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Domain.Repositories;

public interface IDriverRepository : IRepository<Driver>
{
    Task<Driver?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Driver>> ListByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);
}
