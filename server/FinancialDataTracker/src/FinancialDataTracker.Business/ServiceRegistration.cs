using FinancialDataTracker.Business.Abstract;
using FinancialDataTracker.Business.Abstract.ExternalServices;
using FinancialDataTracker.Business.Concrete;
using FinancialDataTracker.Business.Concrete.ExternalServices;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Abstract.ExternalServices;
using FinancialDataTracker.DataAccess.Concrete;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialDataTracker.Business;

public static class ServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IStockSyncService, StockSyncService>();
        services.AddScoped<IStockService, StockManager>();
        services.AddScoped<IWatchlistService, WatchlistManager>();

        return services;
    }
}
