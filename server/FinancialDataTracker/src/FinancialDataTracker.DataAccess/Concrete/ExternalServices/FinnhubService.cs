using System.Text.Json;
using FinancialDataTracker.DataAccess.Abstract.ExternalServices;
using FinancialDataTracker.DataAccess.Concrete.Context;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices.Options;
using FinancialDataTracker.Entities.Concrete;
using FinancialDataTracker.Entities.Concrete.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinancialDataTracker.DataAccess.Concrete.ExternalServices;

public sealed class FinnhubService(IOptions<FinnhubApiCredentials> credentials, ApplicationDbContext context) : IFinnhubService
{
    private readonly ApplicationDbContext _context = context;
    private readonly FinnhubApiCredentials _credentials = credentials.Value;

    internal async Task<string> GetStockDetailsAsync()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{_credentials.BaseUrl}/stock/symbol?exchange=US&token={_credentials.ApiKey}");
        var stocksData = await response.Content.ReadAsStringAsync();

        return stocksData;
    }

    public async Task WriteDatabaseAsync()
    {
        var stocksData = await GetStockDetailsAsync();
        var dtos = JsonSerializer.Deserialize<List<StockDto>>(stocksData) ?? new List<StockDto>();


        #region duplicate check
        
        var symbols = dtos
            .Select(x => x.Symbol)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var existingSymbols = await _context.Stocks
            .Select(x => x.StockSymbol.Symbol)
            .ToListAsync();        

        #endregion



        var newStocks = dtos.
            Where(x => !string.IsNullOrWhiteSpace(x.Symbol) && !existingSymbols.Contains(x.Symbol))
            .Select(dto => new Stock
            {
                StockSymbol = new StockDetails(dto.Symbol, dto.DisplaySymbol, dto.Description, dto.Currency,dto.Type)
            }).ToList();        

        await _context.Stocks.AddRangeAsync(newStocks);
        await _context.SaveChangesAsync();
    }

    
}
