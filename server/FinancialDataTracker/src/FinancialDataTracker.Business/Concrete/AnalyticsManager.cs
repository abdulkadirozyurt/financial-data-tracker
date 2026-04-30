using FinancialDataTracker.Business.Abstract;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.Entities.Concrete.DTOs;

namespace FinancialDataTracker.Business.Concrete;

public sealed class AnalyticsManager(IQuoteSnapshotRepository quoteSnapshotRepository) : IAnalyticsService
{
    public async Task<IReadOnlyList<TopMoverDto>> GetTopMoversAsync(string direction, int limit = 5, CancellationToken cancellationToken = default)
    {
        var latestSnapshots = await quoteSnapshotRepository.GetLatestSnapshotsAsync(cancellationToken);

        var mappedDtos = latestSnapshots.Select(s => new TopMoverDto(
              s.Symbol,
              s.Stock.StockDetails.DisplaySymbol,
              s.Stock.StockDetails.Description,
              s.Quote.CurrentPrice,
              s.Quote.PreviousClosePrice,
              s.Quote.Change,
              s.Quote.PercentChange,
              s.FetchedAtUtc));

        var normalizedDirection = direction.Trim().ToLowerInvariant();

        var result = normalizedDirection == "losers"
            ? mappedDtos.OrderBy(x => x.PercentChange).Take(limit).ToList()
            : mappedDtos.OrderByDescending(x => x.PercentChange).Take(limit).ToList();

        return result;
    }
}
