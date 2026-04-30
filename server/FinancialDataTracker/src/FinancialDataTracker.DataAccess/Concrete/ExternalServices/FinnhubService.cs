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


    public async Task<IReadOnlyList<StockDto>> GetStockDetailsAsync(string? exchange = "US", CancellationToken cancellationToken = default)
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
        using var response = await httpClient.GetAsync(endpoint, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        EnsureSuccess(response, content, $"Quote for {symbol}");

        return JsonSerializer.Deserialize<FinnhubQuoteDto>(content, JsonOptions)
            ?? throw new ExternalServiceException($"Finnhub returned an empty quote response for {symbol}.");
    }

    private static void EnsureSuccess(HttpResponseMessage response, string content, string operation)
    {
        if (response.IsSuccessStatusCode)
            return;
        throw new ExternalServiceException($"Finnhub {operation} request failed with {(int)response.StatusCode}: {content}");
    }
}