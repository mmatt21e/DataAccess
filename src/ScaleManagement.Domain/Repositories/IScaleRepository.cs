using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Repositories;

public interface IScaleRepository : IRepository<Scale>
{
    Task<Scale?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Scale>> ListByTypeAsync(ScaleType scaleType, bool activeOnly = true, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Scale>> ListActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>Scales whose next calibration is due on/before <paramref name="dueBeforeUtc"/>.</summary>
    Task<IReadOnlyList<Scale>> ListDueForCalibrationAsync(DateTimeOffset dueBeforeUtc, CancellationToken cancellationToken = default);
}
