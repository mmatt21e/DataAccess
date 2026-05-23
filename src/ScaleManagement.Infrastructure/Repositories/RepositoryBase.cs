using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

/// <summary>
/// Default <see cref="IRepository{T}"/> implementation over EF Core. Reads that
/// flow through the entity <see cref="DbSet{TEntity}"/> are tracked so callers
/// can fetch-modify-save; <see cref="Query"/> is no-tracking for projections and
/// reporting. Tenant and soft-delete global filters are applied automatically by
/// the <see cref="ScaleManagementDbContext"/>.
/// </summary>
public class RepositoryBase<T> : IRepository<T> where T : class, IEntity
{
    protected readonly ScaleManagementDbContext Context;
    protected readonly DbSet<T> Set;

    public RepositoryBase(ScaleManagementDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
        => await Set.ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await Set.Where(predicate).ToListAsync(cancellationToken);

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => Set.FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => Set.AnyAsync(predicate, cancellationToken);

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => predicate is null ? Set.CountAsync(cancellationToken) : Set.CountAsync(predicate, cancellationToken);

    public IQueryable<T> Query() => Set.AsNoTracking();

    public IQueryable<T> QueryIncludingDeleted()
    {
        // IgnoreQueryFilters drops every global filter, so re-apply tenant scoping
        // by hand to preserve isolation while exposing soft-deleted rows.
        IQueryable<T> query = Set.IgnoreQueryFilters().AsNoTracking();

        if (typeof(ITenantScoped).IsAssignableFrom(typeof(T)) && Context.CurrentTenantId is Guid tenantId)
            query = query.Where(e => EF.Property<Guid>(e, nameof(ITenantScoped.TenantId)) == tenantId);

        return query;
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await Set.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => await Set.AddRangeAsync(entities, cancellationToken);

    public void Update(T entity) => Set.Update(entity);

    public void Remove(T entity) => Set.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) => Set.RemoveRange(entities);
}
