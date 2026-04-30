using FinancialDataTracker.Entities.Concrete.DTOs;

namespace FinancialDataTracker.Business.Abstract;

public interface IQuoteSnapshotService
{
    Task<SyncWatchlistQuotesResultDto> SyncWatchlistQuotesAsync(Guid watchlistId, CancellationToken cancellationToken = default);
}
