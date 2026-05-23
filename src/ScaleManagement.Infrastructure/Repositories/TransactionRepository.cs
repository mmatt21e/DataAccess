using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Entities;
using ScaleManagement.Domain.Enums;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

public sealed class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
{
    public TransactionRepository(ScaleManagementDbContext context) : base(context) { }

    public Task<Transaction?> GetByNumberAsync(string transactionNumber, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(t => t.TransactionNumber == transactionNumber, cancellationToken);

    public Task<Transaction?> GetWithMeasurementsAsync(Guid id, CancellationToken cancellationToken = default)
        => Set.Include(t => t.Measurements).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Transaction>> ListByScaleAsync(Guid scaleId, CancellationToken cancellationToken = default)
        => await Set.Where(t => t.ScaleId == scaleId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Transaction>> ListOpenAsync(CancellationToken cancellationToken = default)
        => await Set.Where(t => t.Status == TransactionStatus.Open)
            .OrderBy(t => t.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<Transaction?> GetOpenForTruckAsync(Guid truckId, CancellationToken cancellationToken = default)
        => Set.Where(t => t.TruckId == truckId && t.Status == TransactionStatus.Open)
            .OrderByDescending(t => t.FirstWeighedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<long> GetMaxSequenceAsync(CancellationToken cancellationToken = default)
    {
        // Transaction numbers are issued as zero-padded sequences; the max existing
        // numeric value lets a service allocate the next ticket. Parsing is done in
        // memory on the (small) max candidate set to stay provider-agnostic.
        var numbers = await Set.Select(t => t.TransactionNumber).ToListAsync(cancellationToken);
        long max = 0;
        foreach (var n in numbers)
        {
            if (long.TryParse(n, out var value) && value > max)
                max = value;
        }
        return max;
    }

    public async Task<PagedResult<Transaction>> SearchAsync(TransactionQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Transaction> q = Set.AsNoTracking();

        if (query.ScaleId is { } scaleId) q = q.Where(t => t.ScaleId == scaleId);
        if (query.ScaleType is { } scaleType) q = q.Where(t => t.ScaleType == scaleType);
        if (query.CompanyId is { } companyId) q = q.Where(t => t.CompanyId == companyId);
        if (query.MaterialId is { } materialId) q = q.Where(t => t.MaterialId == materialId);
        if (query.TruckId is { } truckId) q = q.Where(t => t.TruckId == truckId);
        if (query.DriverId is { } driverId) q = q.Where(t => t.DriverId == driverId);
        if (query.Status is { } status) q = q.Where(t => t.Status == status);
        if (query.Direction is { } direction) q = q.Where(t => t.Direction == direction);
        if (query.FromUtc is { } fromUtc) q = q.Where(t => t.CreatedAtUtc >= fromUtc);
        if (query.ToUtc is { } toUtc) q = q.Where(t => t.CreatedAtUtc <= toUtc);

        if (!string.IsNullOrWhiteSpace(query.TextSearch))
        {
            var term = query.TextSearch.Trim();
            q = q.Where(t =>
                t.TransactionNumber.Contains(term) ||
                (t.ReferenceNumber != null && t.ReferenceNumber.Contains(term)) ||
                (t.TicketNumber != null && t.TicketNumber.Contains(term)));
        }

        var total = await q.CountAsync(cancellationToken);

        q = query.Descending
            ? q.OrderByDescending(t => t.CreatedAtUtc)
            : q.OrderBy(t => t.CreatedAtUtc);

        var pageSize = query.PageSize <= 0 ? 50 : query.PageSize;
        var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;

        var items = await q
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Transaction>(items, total, pageNumber, pageSize);
    }
}
