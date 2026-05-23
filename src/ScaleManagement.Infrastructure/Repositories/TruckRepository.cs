using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

public sealed class TruckRepository : RepositoryBase<Truck>, ITruckRepository
{
    public TruckRepository(ScaleManagementDbContext context) : base(context) { }

    public Task<Truck?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(t => t.LicensePlate == licensePlate, cancellationToken);

    public async Task<IReadOnlyList<Truck>> ListByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default)
        => await Set.Where(t => t.CompanyId == companyId)
            .OrderBy(t => t.LicensePlate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Truck>> ListWithExpiringTareAsync(DateTimeOffset beforeUtc, CancellationToken cancellationToken = default)
        => await Set.Where(t => t.StoredTareWeight != null && t.TareValidUntilUtc != null && t.TareValidUntilUtc <= beforeUtc)
            .OrderBy(t => t.TareValidUntilUtc)
            .ToListAsync(cancellationToken);
}
