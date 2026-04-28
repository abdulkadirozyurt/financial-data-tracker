using System;
using System.Text.Json.Serialization;
using FinancialDataTracker.Entities.Abstract;

namespace FinancialDataTracker.Entities.Concrete;

public sealed class Stock : Entity
{
    public StockDetails StockSymbol { get; set; } = default!;
}

public sealed record StockDetails(
    string Symbol,
    string DisplaySymbol,
    string Description,
    string Currency,
    string Type);