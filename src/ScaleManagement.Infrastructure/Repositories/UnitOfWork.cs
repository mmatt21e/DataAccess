using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Repositories;
using ScaleManagement.Infrastructure.Persistence;

namespace ScaleManagement.Infrastructure.Repositories;

/// <summary>
/// Default <see cref="IUnitOfWork"/>. Shares a single <see cref="ScaleManagementDbContext"/>
/// (and therefore one change tracker / transaction scope) across all repositories
/// it exposes, so multi-aggregate operations commit atomically.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ScaleManagementDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _genericRepositories = new();

    public UnitOfWork(ScaleManagementDbContext context)
    {
        _context = context;

        Scales = new ScaleRepository(context);
        Transactions = new TransactionRepository(context);
        Materials = new MaterialRepository(context);
        Companies = new CompanyRepository(context);
        Drivers = new DriverRepository(context);
        Trucks = new TruckRepository(context);
        Users = new UserRepository(context);
        Queues = new QueueRepository(context);
        AuditConfigurations = new AuditConfigurationRepository(context);
        AuditLogs = new AuditLogRepository(context);
    }

    public IScaleRepository Scales { get; }
    public ITransactionRepository Transactions { get; }
    public IMaterialRepository Materials { get; }
    public ICompanyRepository Companies { get; }
    public IDriverRepository Drivers { get; }
    public ITruckRepository Trucks { get; }
    public IUserRepository Users { get; }
    public IQueueRepository Queues { get; }
    public IAuditConfigurationRepository AuditConfigurations { get; }
    public IAuditLogRepository AuditLogs { get; }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity
        => (IRepository<TEntity>)_genericRepositories.GetOrAdd(
            typeof(TEntity),
            _ => new RepositoryBase<TEntity>(_context));

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        // The execution strategy makes the whole transaction retry-safe against
        // transient (e.g. SQL Server) failures.
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await operation(cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public ValueTask DisposeAsync() => _context.DisposeAsync();
}
