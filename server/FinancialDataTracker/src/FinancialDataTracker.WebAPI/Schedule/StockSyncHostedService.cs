using FinancialDataTracker.Business.Abstract.ExternalServices;
using TimeZoneConverter;

namespace FinancialDataTracker.WebAPI.Schedule;

public sealed class StockSyncHostedService(
    IServiceProvider serviceProvider,
    ILogger<StockSyncHostedService> logger
    ) : BackgroundService
{
    private static readonly TimeZoneInfo _newYorkTimeZone = TZConvert.GetTimeZoneInfo("America/New_York");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await RunOnceAsync(stoppingToken); // app initialization

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRun();
                logger.LogInformation($"Next stock sync scheduled at {DateTime.UtcNow + delay} (UTC), waiting {delay}.");
                await Task.Delay(delay, stoppingToken);
                await RunOnceAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Stock sync hosted service is stopping due to cancellation.");
        }
    }

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var stockSyncService = scope.ServiceProvider.GetRequiredService<IStockSyncService>();

        const int maxAttempts = 3;
        var backoff = TimeSpan.FromSeconds(5);

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                logger.LogInformation($"Starting stock sync attempt {attempt}");
                await stockSyncService.SyncStockDataAsync();
                logger.LogInformation("Stock sync succeeded.");
                return;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(ex, $"Stock sync failed on attempt {attempt}, will retry after {backoff}");

                await Task.Delay(backoff, cancellationToken);

                backoff *= 2;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stock sync failed on final attempt.");
                return;
            }
        }
    }

    private static TimeSpan GetDelayUntilNextRun()
    {
        var newyorkLocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _newYorkTimeZone);
        var nextLocalRunTimeNewyork = newyorkLocalNow.Date.AddHours(17).AddSeconds(10); // next run time in New York local time (5:00:10 PM)

        if (newyorkLocalNow >= nextLocalRunTimeNewyork)
            nextLocalRunTimeNewyork = nextLocalRunTimeNewyork.AddDays(1);

        var delay = TimeZoneInfo.ConvertTimeToUtc(nextLocalRunTimeNewyork, _newYorkTimeZone) - DateTime.UtcNow;

        return delay > TimeSpan.Zero ? delay : TimeSpan.FromSeconds(5);
    }
}