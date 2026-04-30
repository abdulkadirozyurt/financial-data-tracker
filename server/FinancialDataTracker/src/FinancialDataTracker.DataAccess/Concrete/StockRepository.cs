using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Concrete.Context;
using FinancialDataTracker.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace FinancialDataTracker.DataAccess.Concrete;

// Repository Pattern: EF Core query details are isolated from business services.
public sealed class StockRepository(ApplicationDbContext dbContext) : IStockRepository
{
    public async Task AddRangeAsync(IEnumerable<Stock> stocks, CancellationToken cancellationToken = default)
    {
        await dbContext.Stocks.AddRangeAsync(stocks, cancellationToken);
    }

    public Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
    {
        search = search?.Trim();
        var query = dbContext.Stocks.AsQueryable();
        if (string.IsNullOrWhiteSpace(search))
            return query.CountAsync(cancellationToken);

        query = query.Where(s =>
        s.StockDetails.Symbol.Contains(search) ||
        s.StockDetails.DisplaySymbol.Contains(search) ||
        (s.StockDetails.Description != null && s.StockDetails.Description.Contains(search)));

        return query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Stock>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Stocks.ToListAsync(cancellationToken);
    }

    public async Task<Stock?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default)
    {
        return await dbContext.Stocks.Where(s => s.StockDetails.Symbol == symbol.Trim()).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Stock>> GetPagedAsync(string? search, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        search = search?.Trim();
        var query = dbContext.Stocks.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
            s.StockDetails.Symbol.Contains(search) ||
            s.StockDetails.DisplaySymbol.Contains(search) ||
            (s.StockDetails.Description != null && s.StockDetails.Description.Contains(search)));
        }

        return await query.AsNoTracking()
            .OrderBy(s => s.StockDetails.Symbol)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
