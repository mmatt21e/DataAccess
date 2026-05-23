using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Enums;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

public sealed class ScaleRepository : RepositoryBase<Scale>, IScaleRepository
{
    public ScaleRepository(ScaleManagementDbContext context) : base(context) { }

    public Task<Scale?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(s => s.Code == code, cancellationToken);

    public async Task<IReadOnlyList<Scale>> ListByTypeAsync(ScaleType scaleType, bool activeOnly = true, CancellationToken cancellationToken = default)
        => await Set.Where(s => s.ScaleType == scaleType && (!activeOnly || s.IsActive))
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Scale>> ListActiveAsync(CancellationToken cancellationToken = default)
        => await Set.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Scale>> ListDueForCalibrationAsync(DateTimeOffset dueBeforeUtc, CancellationToken cancellationToken = default)
        => await Set.Where(s => s.IsActive && s.NextCalibrationDueUtc != null && s.NextCalibrationDueUtc <= dueBeforeUtc)
            .OrderBy(s => s.NextCalibrationDueUtc)
            .ToListAsync(cancellationToken);
}
