using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Enums;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

public sealed class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(ScaleManagementDbContext context) : base(context) { }

    public Task<Company?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

    public async Task<IReadOnlyList<Company>> ListByTypeAsync(CompanyType type, CancellationToken cancellationToken = default)
        // Bitwise match so a Customer|Carrier company is returned for either filter.
        => await Set.Where(c => (c.Type & type) == type)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
}
