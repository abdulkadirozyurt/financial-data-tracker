using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Abstract.ExternalServices;
using FinancialDataTracker.DataAccess.Concrete;
using FinancialDataTracker.DataAccess.Concrete.Context;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace FinancialDataTracker.DataAccess;

public static class ServiceRegistration
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.Configure<FinnhubApiCredentials>(configuration.GetSection("FinnhubApiCredentials"));

        services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("SqlServer")));


        services.AddScoped<IFinnhubService, FinnhubService>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<IWatchlistRepository, WatchlistRepository>();


        return services;
    }
}
