using FinancialDataTracker.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialDataTracker.Entities.Concrete;

public sealed class StockQuoteSnapshot : Entity
{
    public Guid StockId { get; set; }
    public string Symbol { get; set; } = default!;
    public Stock Stock { get; set; } = default!;
    public StockQuote Quote { get; set; } = default!;
    public DateTime FetchedAtUtc { get; set; } = DateTime.UtcNow;
}


public sealed record StockQuote(
    decimal CurrentPrice,
    decimal OpenPrice,
    decimal HighPrice,
    decimal LowPrice,
    decimal PreviousClosePrice,
    decimal Change,
    decimal PercentChange,
    DateTime? FinnhubTimestampUtc
);