using FinancialDataTracker.Entities.Concrete.DTOs;

namespace FinancialDataTracker.Business.Abstract;

public interface IStockService
{
    Task<PagedResultDto<StockListItemDto>> GetStockListAsync(
        string? search, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
