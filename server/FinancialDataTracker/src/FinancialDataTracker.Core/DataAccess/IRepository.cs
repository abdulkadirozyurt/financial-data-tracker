using System.Linq.Expressions;

namespace FinancialDataTracker.Core.DataAccess;

public interface IRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default);
    Task<T?> GetAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);
    Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default);
    Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetPagedAsync<TKey>(
        Expression<Func<T, bool>>? filter,
        Expression<Func<T, TKey>> orderBy,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(
      Expression<Func<T, bool>>? filter = null,
      CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
