using FinancialDataTracker.Core.DataAccess;
using FinancialDataTracker.Entities.Concrete;

namespace FinancialDataTracker.DataAccess.Abstract;

public interface IWatchlistRepository : IRepository<Watchlist>
{
    Task<Watchlist?> GetByIdWithStocksAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Watchlist>> GetAllWithStocksAsync(CancellationToken cancellationToken = default);
}
