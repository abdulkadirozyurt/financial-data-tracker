using FinancialDataTracker.Core.DataAccess.ExternalServices;
using FinancialDataTracker.Entities.Concrete.DTOs;

namespace FinancialDataTracker.DataAccess.Abstract.ExternalServices;

public interface IFinnhubService : IExternalService
{
    Task<IReadOnlyList<StockDto>> GetStockDetailsAsync(string exchange, CancellationToken cancellationToken = default);
    Task<FinnhubQuoteDto> GetQuoteAsync(string symbol, CancellationToken cancellationToken = default);
}
