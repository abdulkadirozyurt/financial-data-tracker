using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Abstract.ExternalServices;
using FinancialDataTracker.DataAccess.Concrete;
using FinancialDataTracker.DataAccess.Concrete.Context;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialDataTracker.DataAccess;

public static class ServiceRegistration
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FinnhubApiCredentials>(configuration.GetSection("FinnhubApiCredentials"));

        services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("SqlServer")));


        services.AddScoped<IFinnhubService, FinnhubService>();
        services.AddScoped<IStockDal, StockDal>();


        return services;
    }
}
