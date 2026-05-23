using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

/// <summary>
/// Read-only access to the audit trail. AuditLog is not tenant filtered at the
/// model level, so this repository scopes by tenant explicitly when a tenant id
/// is supplied on the query.
/// </summary>
public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly ScaleManagementDbContext _context;

    public AuditLogRepository(ScaleManagementDbContext context) => _context = context;

    public async Task<IReadOnlyList<AuditLog>> ListForEntityAsync(string entityName, string entityId, CancellationToken cancellationToken = default)
        => await _context.AuditLogs.AsNoTracking()
            .Where(a => a.EntityName == entityName && a.EntityId == entityId)
            .OrderByDescending(a => a.TimestampUtc)
            .ToListAsync(cancellationToken);

    public async Task<PagedResult<AuditLog>> SearchAsync(AuditLogQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLog> q = _context.AuditLogs.AsNoTracking();

        if (query.TenantId is { } tenantId) q = q.Where(a => a.TenantId == tenantId);
        if (!string.IsNullOrWhiteSpace(query.EntityName)) q = q.Where(a => a.EntityName == query.EntityName);
        if (!string.IsNullOrWhiteSpace(query.UserId)) q = q.Where(a => a.UserId == query.UserId);
        if (query.FromUtc is { } fromUtc) q = q.Where(a => a.TimestampUtc >= fromUtc);
        if (query.ToUtc is { } toUtc) q = q.Where(a => a.TimestampUtc <= toUtc);

        var total = await q.CountAsync(cancellationToken);

        var pageSize = query.PageSize <= 0 ? 100 : query.PageSize;
        var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;

        var items = await q
            .OrderByDescending(a => a.TimestampUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLog>(items, total, pageNumber, pageSize);
    }
}
