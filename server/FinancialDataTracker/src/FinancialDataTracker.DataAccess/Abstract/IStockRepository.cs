using FinancialDataTracker.Core.DataAccess;
using FinancialDataTracker.Entities.Concrete;

namespace FinancialDataTracker.DataAccess.Abstract;

public interface IStockRepository : IRepository<Stock>
{
    Task<IReadOnlyList<Stock>> GetAllForSyncAsync(CancellationToken cancellationToken = default);
    Task<Stock?> GetTrackedBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
}
