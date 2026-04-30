using FinancialDataTracker.Core.DataAccess;
using FinancialDataTracker.Entities.Concrete;

namespace FinancialDataTracker.DataAccess.Abstract;

public interface IQuoteSnapshotRepository : IRepository<StockQuoteSnapshot>
{
    Task<IReadOnlyList<StockQuoteSnapshot>> GetLatestSnapshotsAsync(CancellationToken cancellationToken = default);
}
