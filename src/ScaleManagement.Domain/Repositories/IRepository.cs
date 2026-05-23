using System.Linq.Expressions;
using ScaleManagement.Domain.Common;

namespace ScaleManagement.Domain.Repositories;

/// <summary>
/// Generic persistence contract shared by every entity. Entity-specific
/// repositories extend this with intent-revealing query methods. Note that
/// mutating methods do not persist on their own — call
/// <see cref="IUnitOfWork.SaveChangesAsync"/> to commit, so that several
/// repository operations can participate in one atomic transaction.
/// </summary>
/// <typeparam name="T">The entity type managed by the repository.</typeparam>
public interface IRepository<T> where T : class, IEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a composable, no-tracking query for read scenarios that need
    /// projection, paging or includes beyond the convenience methods. The active
    /// tenant and soft-delete global filters are always applied.
    /// </summary>
    IQueryable<T> Query();

    /// <summary>As <see cref="Query"/> but ignores the soft-delete filter (administrative recovery views).</summary>
    IQueryable<T> QueryIncludingDeleted();

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Update(T entity);

    /// <summary>
    /// Marks the entity for removal. For soft-deletable entities this is converted
    /// to a flag update during save; otherwise it is a physical delete.
    /// </summary>
    void Remove(T entity);

    void RemoveRange(IEnumerable<T> entities);
}
