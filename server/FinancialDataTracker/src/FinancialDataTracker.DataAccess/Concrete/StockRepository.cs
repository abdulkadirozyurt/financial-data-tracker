using FinancialDataTracker.Core.DataAccess;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Concrete.Context;
using FinancialDataTracker.Entities.Abstract;
using FinancialDataTracker.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinancialDataTracker.DataAccess.Concrete;

// Repository Pattern: EF Core query details are isolated from business services.
public sealed class StockRepository(ApplicationDbContext context) : Repository<Stock, ApplicationDbContext>(context), IStockRepository
{
    public async Task<IReadOnlyList<Stock>> GetAllForSyncAsync(CancellationToken cancellationToken = default)
    {
        return await context.Stocks.AsTracking().ToListAsync();
    }
}
