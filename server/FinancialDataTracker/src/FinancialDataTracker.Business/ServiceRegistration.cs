using FinancialDataTracker.Business.Abstract;
using FinancialDataTracker.Business.Abstract.ExternalServices;
using FinancialDataTracker.Business.Concrete;
using FinancialDataTracker.Business.Concrete.ExternalServices;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialDataTracker.Business;

public static class ServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IStockSyncService, StockSyncService>();
        services.AddScoped<IStockService, StockManager>();
        services.AddScoped<IQuoteSnapshotService, QuoteSnapshotManager>();
        services.AddScoped<IWatchlistService, WatchlistManager>();

        return services;
    }
}
