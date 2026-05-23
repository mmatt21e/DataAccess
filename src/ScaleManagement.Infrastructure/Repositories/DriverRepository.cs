using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

public sealed class DriverRepository : RepositoryBase<Driver>, IDriverRepository
{
    public DriverRepository(ScaleManagementDbContext context) : base(context) { }

    public Task<Driver?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber, cancellationToken);

    public async Task<IReadOnlyList<Driver>> ListByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default)
        => await Set.Where(d => d.CompanyId == companyId)
            .OrderBy(d => d.LastName).ThenBy(d => d.FirstName)
            .ToListAsync(cancellationToken);
}
