using FinancialDataTracker.Business.Abstract.ExternalServices;
using FinancialDataTracker.DataAccess.Abstract.ExternalServices;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices;

namespace FinancialDataTracker.Business.Concrete.ExternalServices;

public sealed class StockSyncService(IFinnhubService finnhubService) : IStockSyncService
{
    public async Task SyncStockDataAsync()
    {
        await finnhubService.WriteDatabaseAsync();
    }
}
