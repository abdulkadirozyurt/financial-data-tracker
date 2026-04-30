using FinancialDataTracker.Business.Abstract;
using FinancialDataTracker.Core.Exceptions;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.Entities.Concrete.DTOs;

namespace FinancialDataTracker.Business.Concrete;

public sealed class WatchlistManager(
    IWatchlistRepository watchlistRepository,
    IStockRepository stockRepository) : IWatchlistService
{
    public async Task<WatchlistDto> AddStockAsync(Guid watchlistId, string symbol, CancellationToken cancellationToken)
    {
        var watchlist = await watchlistRepository.GetByIdWithStocksAsync(watchlistId, cancellationToken);
        var stock = await stockRepository.GetTrackedBySymbolAsync(symbol, cancellationToken);

        if (watchlist is null) throw new NotFoundException($"Watchlist with ID {watchlistId} not found.");
        if (stock is null) throw new NotFoundException($"Stock with symbol {symbol} not found.");

        bool isStockExists = watchlist.Stocks.Any(s => s.Id == stock.Id);
        if (isStockExists) throw new ConflictException($"Stock with symbol {symbol} already exists in the watchlist.");

        watchlist.Stocks.Add(stock);
        await watchlistRepository.SaveChangesAsync(cancellationToken);

        return MapWatchlistDto(watchlist);
    }

    public async Task<WatchlistDto> CreateAsync(CreateWatchlistRequest watchlistRequest, CancellationToken cancellationToken)
    {
        var name = watchlistRequest.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Watchlist name cannot be empty.");

        var watchlist = new Entities.Concrete.Watchlist
        {
            Name = name
        };

        await watchlistRepository.AddAsync(watchlist, cancellationToken);
        await watchlistRepository.SaveChangesAsync(cancellationToken);

        return MapWatchlistDto(watchlist);
    }

    public async Task<IReadOnlyList<WatchlistDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var watchlists = await watchlistRepository.GetAllWithStocksAsync(cancellationToken: cancellationToken);
        return watchlists.Select(MapWatchlistDto).ToList();
    }

    public async Task<WatchlistDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var watchlist = await watchlistRepository.GetByIdWithStocksAsync(id, cancellationToken);
        return watchlist is null ? null : MapWatchlistDto(watchlist);
    }

    public async Task RemoveStockAsync(Guid watchlistId, string symbol, CancellationToken cancellationToken)
    {
        var watchlist = await watchlistRepository.GetByIdWithStocksAsync(watchlistId, cancellationToken);
        if (watchlist is null) throw new NotFoundException($"Watchlist with ID {watchlistId} not found.");

        var stock = await stockRepository.GetTrackedBySymbolAsync(symbol, cancellationToken);
        if (stock is null) throw new NotFoundException($"Stock with symbol {symbol} not found.");

        var existingStock = watchlist.Stocks.FirstOrDefault(s => s.Id == stock.Id);
        if (existingStock is null) throw new NotFoundException($"Stock with symbol {symbol} not found in the watchlist.");

        watchlist.Stocks.Remove(existingStock);
        await watchlistRepository.SaveChangesAsync(cancellationToken);
    }

    private static WatchlistDto MapWatchlistDto(Entities.Concrete.Watchlist watchlist)
    {
        return new WatchlistDto(
            watchlist.Id,
            watchlist.Name,
            watchlist.Stocks.Select(s => new WatchlistStockDto(
                s.Id,
                s.StockDetails.Symbol,
                s.StockDetails.DisplaySymbol,
                s.StockDetails.Description,
                s.StockDetails.Currency,
                s.StockDetails.Type,
                null)).ToList());
    }
}
