using System;
using System.Collections.Generic;
using System.Text;
using FinancialDataTracker.Business.Abstract.ExternalServices;
using FinancialDataTracker.Business.Concrete;
using FinancialDataTracker.Business.Concrete.ExternalServices;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Abstract.ExternalServices;
using FinancialDataTracker.DataAccess.Concrete;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialDataTracker.Business;

public static class ServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IStockSyncService, StockSyncService>();
        return services;
    }
}
