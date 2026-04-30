using FinancialDataTracker.Core.DataAccess.ExternalServices;

namespace FinancialDataTracker.Business.Abstract.ExternalServices;

public interface IStockSyncService: IExternalService
{
    Task SyncStockDataAsync(string? exchange="US");
}
