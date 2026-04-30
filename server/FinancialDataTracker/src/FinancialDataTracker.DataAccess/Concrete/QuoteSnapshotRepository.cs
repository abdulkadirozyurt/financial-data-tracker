using FinancialDataTracker.Core.DataAccess;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Concrete.Context;
using FinancialDataTracker.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace FinancialDataTracker.DataAccess.Concrete;

public sealed class QuoteSnapshotRepository(ApplicationDbContext context) : Repository<StockQuoteSnapshot, ApplicationDbContext>(context), IQuoteSnapshotRepository
{
    public async Task<IReadOnlyList<StockQuoteSnapshot>> GetLatestSnapshotsAsync(CancellationToken cancellationToken = default)
    {
        var latestSnapshotIds = await context.StockQuoteSnapshots
            .GroupBy(x => x.StockId)
            .Select(x => x.OrderByDescending(x => x.FetchedAtUtc)
            .Select(x => x.Id)
            .First())
            .ToListAsync(cancellationToken);

        return await context.StockQuoteSnapshots
            .Where(x => latestSnapshotIds.Contains(x.Id))
            .Include(x => x.Stock)
            .ToListAsync(cancellationToken);
    }
}
