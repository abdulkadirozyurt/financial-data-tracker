using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinancialDataTracker.Core.DataAccess;

public class Repository<TEntity, TContext>(TContext context) : IRepository<TEntity>
where TEntity : class
where TContext : DbContext
{
    private readonly DbSet<TEntity> _set = context.Set<TEntity>();


    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _set.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _set.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        return await _set.CountAsync(filter ?? (_ => true), cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        return await _set.Where(filter ?? (_ => true)).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _set.Where(filter).AsNoTracking().SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetPagedAsync<TKey>(
        Expression<Func<TEntity, bool>>? filter,
        Expression<Func<TEntity, TKey>> orderBy,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _set.AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        return await query.OrderBy(orderBy)
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize)
                     .AsNoTracking()
                     .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
