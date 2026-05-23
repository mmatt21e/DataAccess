using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Domain.Repositories;

public interface IQueueRepository : IRepository<Queue>
{
    /// <summary>Loads a queue with its entries ordered by priority then position.</summary>
    Task<Queue?> GetWithEntriesAsync(Guid queueId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Queue>> ListActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>The active queue feeding a particular scale, if configured.</summary>
    Task<Queue?> GetForScaleAsync(Guid scaleId, CancellationToken cancellationToken = default);
}
