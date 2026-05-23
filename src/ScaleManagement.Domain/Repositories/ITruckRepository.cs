using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Domain.Repositories;

public interface ITruckRepository : IRepository<Truck>
{
    Task<Truck?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Truck>> ListByCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Truck>> ListWithExpiringTareAsync(DateTimeOffset beforeUtc, CancellationToken cancellationToken = default);
}
