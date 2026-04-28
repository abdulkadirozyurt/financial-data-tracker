using FinancialDataTracker.Core.DataAccess.ExternalServices;

namespace FinancialDataTracker.DataAccess.Abstract.ExternalServices;

public interface IFinnhubService : IExternalService
{
    Task WriteDatabaseAsync();
}
