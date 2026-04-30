using FinancialDataTracker.Business.Abstract;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.Entities.Concrete;
using FinancialDataTracker.Entities.Concrete.DTOs;
using System.Linq.Expressions;

namespace FinancialDataTracker.Business.Concrete;

public sealed class StockManager(IStockRepository stockRepository) : IStockService
{
    public async Task<PagedResultDto<StockListItemDto>> GetStockListAsync(string? search, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        search = search?.Trim();
        Expression<Func<Stock, bool>>? filter = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            filter = x =>
                x.StockDetails.Symbol.Contains(search) ||
                x.StockDetails.DisplaySymbol.Contains(search) ||
                (x.StockDetails.Description != null && x.StockDetails.Description.Contains(search)) ||
                (x.StockDetails.Type != null && x.StockDetails.Type.Contains(search)) ||
                (x.StockDetails.Currency != null && x.StockDetails.Currency.Contains(search));
        }

        var totalCount = await stockRepository.CountAsync(filter, cancellationToken);

        var stocks = await stockRepository.GetPagedAsync(
            filter,
            orderBy: x => x.StockDetails.Symbol,
            pageNumber,
            pageSize,
            cancellationToken);

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

    public async Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
    {
        search = search?.Trim();
        Expression<Func<Stock, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(search))
        {
            filter = x =>
                x.StockDetails.Symbol.Contains(search) ||
                x.StockDetails.DisplaySymbol.Contains(search) ||
                (x.StockDetails.Description != null && x.StockDetails.Description.Contains(search)) ||
                (x.StockDetails.Type != null && x.StockDetails.Type.Contains(search)) ||
                (x.StockDetails.Currency != null && x.StockDetails.Currency.Contains(search));
        }
        return await stockRepository.CountAsync(filter, cancellationToken);
    }

    public Task<Stock> GetStockDetailBySymbolAsync(string symbol, CancellationToken cancellationToken = default)
    {
        return stockRepository.GetAsync(x => x.StockDetails.Symbol == symbol, cancellationToken);
    }
}
