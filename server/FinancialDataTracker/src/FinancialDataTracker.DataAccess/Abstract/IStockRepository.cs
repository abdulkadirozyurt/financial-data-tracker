using FinancialDataTracker.Entities.Concrete;

namespace FinancialDataTracker.DataAccess.Abstract;

public interface IStockRepository
{
    Task<int> CountAsync(string? search, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stock>> GetPagedAsync(string? search, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stock>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Stock?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Stock> stocks, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
