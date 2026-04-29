using System.Text.Json;
using FinancialDataTracker.Core.Exceptions;
using FinancialDataTracker.DataAccess.Abstract.ExternalServices;
using FinancialDataTracker.DataAccess.Concrete.Context;
using FinancialDataTracker.DataAccess.Concrete.ExternalServices.Options;
using FinancialDataTracker.Entities.Concrete;
using FinancialDataTracker.Entities.Concrete.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinancialDataTracker.DataAccess.Concrete.ExternalServices;

public sealed class FinnhubService(
    IOptions<FinnhubApiCredentials> credentials,
    ApplicationDbContext context,
    HttpClient httpClient
    ) : IFinnhubService
{
    private readonly ApplicationDbContext _context = context;
    private readonly string BaseUrl = credentials.Value.BaseUrl;
    private readonly string ApiKey = credentials.Value.ApiKey;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);


    public async Task<IReadOnlyList<StockDto>> GetStockDetailsAsync(string exchange, CancellationToken cancellationToken = default)
    {
        var endpoint = $"{BaseUrl}/stock/symbol?exchange={exchange}&token={ApiKey}";
        using var response = await httpClient.GetAsync(endpoint, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        EnsureSuccess(response, content, "Stock symbols");

        return JsonSerializer.Deserialize<List<StockDto>>(content) ?? [];
    }

    public async Task<FinnhubQuoteDto> GetQuoteAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var endpoint = $"{BaseUrl}/quote?symbol={symbol}&token={ApiKey}";
        using var response = await httpClient.GetAsync(endpoint, cancellationToken) ;
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        EnsureSuccess(response, content, $"Quote for {symbol}");
        
        return JsonSerializer.Deserialize<FinnhubQuoteDto>(content, JsonOptions) 
            ?? throw new ExternalServiceException($"Finnhub returned an empty quote response for {symbol}.");
    }

    public async Task WriteDatabaseAsync()
    {
        int updatedCount = 0;
        int unchangedCount = 0;
        int missingInDbCount = 0;

        var stocksData = await GetStockDetailsAsync("US");
        var dtos = stocksData ?? new List<StockDto>();
        var normalizedDtos = dtos
            .Where(x => !string.IsNullOrWhiteSpace(x.Symbol))
            .GroupBy(x => x.Symbol, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        List<Stock> toInsert = new List<Stock>();

        var existingStocks = await _context.Stocks.ToListAsync();

        var existingBySymbol = existingStocks.ToDictionary(
            x => x.StockDetails.Symbol,
            StringComparer.OrdinalIgnoreCase);



        foreach (var dto in normalizedDtos)
        {
            if (!existingBySymbol.TryGetValue(dto.Symbol, out var existing))
            {
                // Şu anki verine göre beklenmiyor, ama güvenlik için sayalım
                missingInDbCount++;
                toInsert.Add(new Stock
                {
                    StockDetails = new StockDetails(
                        dto.Symbol,
                        dto.DisplaySymbol,
                        dto.Description,
                        dto.Currency,
                        dto.Type)
                });
                continue;
            }

            bool changed =
                !string.Equals(existing.StockDetails.DisplaySymbol, dto.DisplaySymbol, StringComparison.Ordinal) ||
                !string.Equals(existing.StockDetails.Description, dto.Description, StringComparison.Ordinal) ||
                !string.Equals(existing.StockDetails.Currency, dto.Currency, StringComparison.Ordinal) ||
                !string.Equals(existing.StockDetails.Type, dto.Type, StringComparison.Ordinal);


            if (!changed)
            {
                unchangedCount++;
                continue;
            }

            existing.StockDetails = new StockDetails(
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

    public Task<IReadOnlyList<StockDto>> GetStockSymbolAsync(string exchange, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private static void EnsureSuccess(HttpResponseMessage response, string content, string operation)
    {
        if (response.IsSuccessStatusCode)
            return;
        throw new ExternalServiceException($"Finnhub {operation} request failed with {(int)response.StatusCode}: {content}");
    }



}