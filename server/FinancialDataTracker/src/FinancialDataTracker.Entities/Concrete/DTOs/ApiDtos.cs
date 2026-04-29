using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialDataTracker.Entities.Concrete.DTOs;

public sealed record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount);

public sealed record SyncStockCatalogResultDto(
    string Exchange,
    int ReceivedCount,
    int InsertedCount,
    int UpdatedCount,
    int UnchangedCount,
    int SkippedCount);

public sealed record StockListItemDto(
    Guid Id,
    string Symbol,
    string DisplaySymbol,
    string? Description,
    string? Currency,
    string? Type);

public sealed record CreateWatchlistRequest(string Name);

public sealed record WatchlistDto(
    Guid Id,
    string Name,
    IReadOnlyList<WatchlistStockDto> Stocks);

public sealed record WatchlistStockDto(
    Guid Id,
    string Symbol,
    string DisplaySymbol,
    string? Description,
    string? Currency,
    string? Type,
    QuoteSnapshotDto? LatestQuote);

public sealed record QuoteSnapshotDto(
    Guid Id,
    string Symbol,
    decimal CurrentPrice,
    decimal OpenPrice,
    decimal HighPrice,
    decimal LowPrice,
    decimal PreviousClosePrice,
    decimal Change,
    decimal PercentChange,
    DateTime? FinnhubTimestampUtc,
    DateTime FetchedAtUtc);

public sealed record SyncWatchlistQuotesResultDto(
    Guid WatchlistId,
    string WatchlistName,
    int RequestedCount,
    int StoredCount,
    IReadOnlyList<string> FailedSymbols,
    IReadOnlyList<QuoteSnapshotDto> Snapshots);

public sealed record TopMoverDto(
    string Symbol,
    string DisplaySymbol,
    string? Description,
    decimal CurrentPrice,
    decimal PreviousClosePrice,
    decimal Change,
    decimal PercentChange,
    DateTime FetchedAtUtc);
