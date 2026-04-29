using FinancialDataTracker.Business.Abstract;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.Entities.Concrete.DTOs;

namespace FinancialDataTracker.Business.Concrete;

public sealed class StockManager(IStockRepository stockDal) : IStockService
{
    public async Task<PagedResultDto<StockListItemDto>> GetStockListAsync(string? search, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await stockDal.CountAsync(search, cancellationToken);
        var stocks = await stockDal.GetPagedAsync(search, pageNumber, pageSize, cancellationToken);

        var resultList = new List<StockListItemDto>();
        foreach (var item in stocks)
        {
            resultList.Add(new StockListItemDto(
                item.Id,
                item.StockDetails.Symbol,
                item.StockDetails.DisplaySymbol,
                item.StockDetails.Description,
                item.StockDetails.Currency,
                item.StockDetails.Type
            ));
        }

        return new PagedResultDto<StockListItemDto>(resultList, pageNumber, pageSize, totalCount);
    }
}
