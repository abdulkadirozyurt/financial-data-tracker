using FinancialDataTracker.Business.Abstract;

namespace FinancialDataTracker.WebAPI.Schedule;

public sealed class QuoteSnapshotSyncHostedService(
    IServiceProvider serviceProvider,
    ILogger<QuoteSnapshotSyncHostedService> logger) : BackgroundService
{
    private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan SyncInterval = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan WatchlistDelay = TimeSpan.FromSeconds(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(InitialDelay, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await RunOnceAsync(stoppingToken);

                logger.LogInformation(
                    "Next quote snapshot sync scheduled at {NextRunUtc} UTC.",
                    DateTime.UtcNow.Add(SyncInterval));

                await Task.Delay(SyncInterval, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Quote snapshot sync hosted service is stopping due to cancellation.");
        }
    }

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var watchlistService = scope.ServiceProvider.GetRequiredService<IWatchlistService>();
        var quoteSnapshotService = scope.ServiceProvider.GetRequiredService<IQuoteSnapshotService>();

        var watchlists = await watchlistService.GetAllAsync(cancellationToken);
        var watchlistsWithStocks = watchlists.Where(w => w.Stocks.Count > 0).ToList();

        if (watchlistsWithStocks.Count == 0)
        {
            logger.LogInformation("Quote snapshot sync skipped because there are no watchlists with stocks.");
            return;
        }

        logger.LogInformation(
            "Quote snapshot sync started for {WatchlistCount} watchlists.",
            watchlistsWithStocks.Count);

        var syncedCount = 0;
        var storedCount = 0;
        var failedSymbols = new List<string>();

        foreach (var watchlist in watchlistsWithStocks)
        {
            try
            {
                var result = await quoteSnapshotService.SyncWatchlistQuotesAsync(watchlist.Id, cancellationToken);

                syncedCount++;
                storedCount += result.StoredCount;
                failedSymbols.AddRange(result.FailedSymbols);

                logger.LogInformation(
                    "Quote snapshot sync completed for watchlist {WatchlistId}. Requested: {RequestedCount}, stored: {StoredCount}, failed: {FailedCount}.",
                    result.WatchlistId,
                    result.RequestedCount,
                    result.StoredCount,
                    result.FailedSymbols.Count);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Quote snapshot sync failed for watchlist {WatchlistId}.",
                    watchlist.Id);
            }

            await Task.Delay(WatchlistDelay, cancellationToken);
        }

        logger.LogInformation(
            "Quote snapshot sync finished. Synced watchlists: {SyncedCount}, stored snapshots: {StoredCount}, failed symbols: {FailedSymbolCount}.",
            syncedCount,
            storedCount,
            failedSymbols.Count);
    }
}
