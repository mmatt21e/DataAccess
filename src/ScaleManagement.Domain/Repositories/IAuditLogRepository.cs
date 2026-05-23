using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Domain.Repositories;

/// <summary>
/// Read-oriented access to the append-only audit trail. There is intentionally
/// no public create/update/delete surface: audit entries are written only by the
/// persistence layer's interceptor.
/// </summary>
public interface IAuditLogRepository
{
    Task<IReadOnlyList<AuditLog>> ListForEntityAsync(string entityName, string entityId, CancellationToken cancellationToken = default);

    Task<PagedResult<AuditLog>> SearchAsync(AuditLogQuery query, CancellationToken cancellationToken = default);
}

public sealed class AuditLogQuery
{
    public Guid? TenantId { get; init; }
    public string? EntityName { get; init; }
    public string? UserId { get; init; }
    public DateTimeOffset? FromUtc { get; init; }
    public DateTimeOffset? ToUtc { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 100;
}
