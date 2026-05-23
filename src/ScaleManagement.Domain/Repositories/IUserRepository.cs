using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<User?> GetByExternalAuthIdAsync(string externalAuthId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> ListActiveAsync(CancellationToken cancellationToken = default);
}
