using FinancialDataTracker.Business.Abstract;
using FinancialDataTracker.Core.Exceptions;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Abstract.ExternalServices;
using FinancialDataTracker.Entities.Concrete;
using FinancialDataTracker.Entities.Concrete.DTOs;
using System.Linq;

namespace FinancialDataTracker.Business.Concrete;

public sealed class QuoteSnapshotManager(
    IWatchlistRepository watchlistRepository,
    IQuoteSnapshotRepository quoteSnapshotRepository,
    IFinnhubService finnhubService) : IQuoteSnapshotService
{
    public async Task<SyncWatchlistQuotesResultDto> SyncWatchlistQuotesAsync(Guid watchlistId, CancellationToken cancellationToken = default)
    {
        var watchlist = await watchlistRepository.GetByIdWithStocksAsync(watchlistId, cancellationToken);

        if (watchlist is null) throw new NotFoundException($"Watchlist '{watchlistId}' was not found.");

        if (watchlist.Stocks.Count == 0)
        {
            return new SyncWatchlistQuotesResultDto(
                watchlist.Id,
                watchlist.Name,
                0,
                0,
                [],
                []
                );
        }

        List<StockQuoteSnapshot> snapshotsToInsert = new();
        List<QuoteSnapshotDto> snapshotDtos = new();
        List<string> failedSymbols = new();

        foreach (var stock in watchlist.Stocks)
        {
            var symbol = stock.StockDetails.Symbol;

            try
            {
                var quote = await finnhubService.GetQuoteAsync(symbol, cancellationToken);

                DateTime? finnhubTimestampUtc = quote.Timestamp.HasValue
                  ? DateTimeOffset.FromUnixTimeSeconds(quote.Timestamp.Value).UtcDateTime
                  : null;

                var snapshot = new StockQuoteSnapshot
                {
                    StockId = stock.Id,
                    Symbol = symbol,
                    Stock = stock,
                    Quote = new StockQuote(
                         quote.CurrentPrice,
                         quote.OpenPrice,
                         quote.HighPrice,
                         quote.LowPrice,
                         quote.PreviousClosePrice,
                         quote.Change,
                         quote.PercentChange,
                         finnhubTimestampUtc),
                    FetchedAtUtc = DateTime.UtcNow
                };
                snapshotsToInsert.Add(snapshot);
            }
            catch (ExternalServiceException)
            {
                failedSymbols.Add(symbol);
            }
        }

        if (snapshotsToInsert.Count > 0)
        {
            await quoteSnapshotRepository.AddRangeAsync(snapshotsToInsert, cancellationToken);
            await quoteSnapshotRepository.SaveChangesAsync(cancellationToken);

            snapshotDtos = snapshotsToInsert
                .Select(snapshot => new QuoteSnapshotDto(
                    snapshot.Id,
                    snapshot.Symbol,
                    snapshot.Quote.CurrentPrice,
                    snapshot.Quote.OpenPrice,
                    snapshot.Quote.HighPrice,
                    snapshot.Quote.LowPrice,
                    snapshot.Quote.PreviousClosePrice,
                    snapshot.Quote.Change,
                    snapshot.Quote.PercentChange,
                    snapshot.Quote.FinnhubTimestampUtc,
                    snapshot.FetchedAtUtc))
                .ToList();
        }

        return new SyncWatchlistQuotesResultDto(
            watchlist.Id,
            watchlist.Name,
            watchlist.Stocks.Count,
            snapshotsToInsert.Count,
            failedSymbols,
            snapshotDtos);
    }
}
