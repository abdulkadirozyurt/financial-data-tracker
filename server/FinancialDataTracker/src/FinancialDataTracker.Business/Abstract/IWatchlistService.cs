using FinancialDataTracker.Entities.Concrete.DTOs;

namespace FinancialDataTracker.Business.Abstract;

public interface IWatchlistService
{
    Task<IReadOnlyList<WatchlistDto>> GetAllAsync(CancellationToken cancellationToken=default);
    Task<WatchlistDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken=default);
    Task<WatchlistDto> CreateAsync(CreateWatchlistRequest watchlistRequest, CancellationToken cancellationToken=default);
    Task<WatchlistDto> AddStockAsync(Guid watchlistId, string symbol, CancellationToken cancellationToken=default);
    Task RemoveStockAsync(Guid watchlistId, string symbol, CancellationToken cancellationToken=default);
}
