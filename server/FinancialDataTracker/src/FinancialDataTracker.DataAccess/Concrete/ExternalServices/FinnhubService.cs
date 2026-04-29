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
        int updatedCount = 0;
        int unchangedCount = 0;
        int missingInDbCount = 0;

        var stocksData = await GetStockDetailsAsync();
        var dtos = JsonSerializer.Deserialize<List<StockDto>>(stocksData) ?? new List<StockDto>();
        var normalizedDtos = dtos
            .Where(x => !string.IsNullOrWhiteSpace(x.Symbol))
            .GroupBy(x => x.Symbol, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        List<Stock> toInsert = new List<Stock>();

        var existingStocks = await _context.Stocks.ToListAsync();

        var existingBySymbol = existingStocks.ToDictionary(
            x => x.StockSymbol.Symbol,
            StringComparer.OrdinalIgnoreCase);



        foreach (var dto in normalizedDtos)
        {
            if (!existingBySymbol.TryGetValue(dto.Symbol, out var existing))
            {
                // Şu anki verine göre beklenmiyor, ama güvenlik için sayalım
                missingInDbCount++;
                toInsert.Add(new Stock
                {
                    StockSymbol = new StockDetails(
                        dto.Symbol,
                        dto.DisplaySymbol,
                        dto.Description,
                        dto.Currency,
                        dto.Type)
                });
                continue;
            }

            bool changed =
                !string.Equals(existing.StockSymbol.DisplaySymbol, dto.DisplaySymbol, StringComparison.Ordinal) ||
                !string.Equals(existing.StockSymbol.Description, dto.Description, StringComparison.Ordinal) ||
                !string.Equals(existing.StockSymbol.Currency, dto.Currency, StringComparison.Ordinal) ||
                !string.Equals(existing.StockSymbol.Type, dto.Type, StringComparison.Ordinal);


            if (!changed)
            {
                unchangedCount++;
                continue;
            }

            existing.StockSymbol = new StockDetails(
                dto.Symbol,
                dto.DisplaySymbol,
                dto.Description,
                dto.Currency,
                dto.Type);

            updatedCount++;
        }

        if (toInsert.Count > 0)
            await _context.Stocks.AddRangeAsync(toInsert);

        if (updatedCount > 0 || toInsert.Count > 0)
            await _context.SaveChangesAsync();
    }
}
