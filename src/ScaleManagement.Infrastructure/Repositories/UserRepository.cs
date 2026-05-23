using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

public sealed class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ScaleManagementDbContext context) : base(context) { }

    public Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

    public Task<User?> GetByExternalAuthIdAsync(string externalAuthId, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(u => u.ExternalAuthId == externalAuthId, cancellationToken);

    public async Task<IReadOnlyList<User>> ListActiveAsync(CancellationToken cancellationToken = default)
        => await Set.Where(u => u.IsActive).OrderBy(u => u.DisplayName).ToListAsync(cancellationToken);
}
