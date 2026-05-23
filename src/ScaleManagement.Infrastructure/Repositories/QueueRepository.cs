using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

public sealed class QueueRepository : RepositoryBase<Queue>, IQueueRepository
{
    public QueueRepository(ScaleManagementDbContext context) : base(context) { }

    public Task<Queue?> GetWithEntriesAsync(Guid queueId, CancellationToken cancellationToken = default)
        => Set.Include(q => q.Entries.OrderBy(e => e.Priority).ThenBy(e => e.Position))
            .FirstOrDefaultAsync(q => q.Id == queueId, cancellationToken);

    public async Task<IReadOnlyList<Queue>> ListActiveAsync(CancellationToken cancellationToken = default)
        => await Set.Where(q => q.IsActive).OrderBy(q => q.Name).ToListAsync(cancellationToken);

    public Task<Queue?> GetForScaleAsync(Guid scaleId, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(q => q.ScaleId == scaleId && q.IsActive, cancellationToken);
}
