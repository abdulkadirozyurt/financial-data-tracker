using FinancialDataTracker.Business.Abstract.ExternalServices;
using FinancialDataTracker.DataAccess.Abstract;
using FinancialDataTracker.DataAccess.Abstract.ExternalServices;
using FinancialDataTracker.Entities.Concrete;
using FinancialDataTracker.Entities.Concrete.DTOs;
using Microsoft.Extensions.Logging;

namespace FinancialDataTracker.Business.Concrete.ExternalServices;

public sealed class StockSyncService(
    IFinnhubService finnhubService,
    IStockRepository stockRepository,
    ILogger<StockSyncService> logger) : IStockSyncService
{
    public async Task SyncStockDataAsync()
    {
        logger.LogInformation("US stock catalog sync started.");

        int updatedCount = 0;
        int unchangedCount = 0;
        int newStockCount = 0;

        var stocksData = await finnhubService.GetStockDetailsAsync("US");
        var dtos = stocksData ?? new List<StockDto>();
        var normalizedDtos = dtos
            .Where(x => !string.IsNullOrWhiteSpace(x.Symbol))
            .GroupBy(x => x.Symbol, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        logger.LogInformation(
            "Finnhub stock catalog fetched. Raw count: {RawCount}, normalized count: {NormalizedCount}.",
            dtos.Count,
            normalizedDtos.Count);

        List<Stock> toInsert = new List<Stock>();

        var existingStocks = await stockRepository.GetAllAsync();

        var existingBySymbol = existingStocks.ToDictionary(
            x => x.StockDetails.Symbol,
            StringComparer.OrdinalIgnoreCase);

        foreach (var dto in normalizedDtos)
        {
            if (!existingBySymbol.TryGetValue(dto.Symbol, out var existing))
            {
                newStockCount++;
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
            await stockRepository.AddRangeAsync(toInsert);

        if (updatedCount > 0 || toInsert.Count > 0)
        {
            var affectedRows = await stockRepository.SaveChangesAsync();
            logger.LogInformation(
                "US stock catalog sync persisted changes. Insert candidates: {InsertCount}, updated: {UpdatedCount}, unchanged: {UnchangedCount}, new stocks: {NewStockCount}, affected rows: {AffectedRows}.",
                toInsert.Count,
                updatedCount,
                unchangedCount,
                newStockCount,
                affectedRows);
            return;
        }

        logger.LogInformation(
            "US stock catalog sync completed with no persisted changes. Unchanged: {UnchangedCount}, new stocks: {NewStockCount}.",
            unchangedCount,
            newStockCount);
    }
}
