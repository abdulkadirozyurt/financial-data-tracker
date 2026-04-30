using FinancialDataTracker.Core.DataAccess;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Concrete.Context;
using FinancialDataTracker.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace FinancialDataTracker.DataAccess.Concrete;

public sealed class WatchlistRepository(ApplicationDbContext context)
    : Repository<Watchlist, ApplicationDbContext>(context), IWatchlistRepository
{
    public async Task<IReadOnlyList<Watchlist>> GetAllWithStocksAsync(CancellationToken cancellationToken = default)
    {
        return await context.Watchlists
            .Include(w => w.Stocks)
            .ToListAsync(cancellationToken);
    }

    public async Task<Watchlist?> GetByIdWithStocksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var watchlist = await context.Watchlists
            .Include(w => w.Stocks)
            .SingleOrDefaultAsync(w => w.Id == id, cancellationToken);

        return watchlist;
    }
}
