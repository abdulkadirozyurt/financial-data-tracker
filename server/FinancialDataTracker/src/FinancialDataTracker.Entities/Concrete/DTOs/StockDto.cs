using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FinancialDataTracker.Entities.Concrete.DTOs;

public sealed record StockDto
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("displaySymbol")]
    public      string DisplaySymbol { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }
    [JsonPropertyName("figi")]
    string Figi { get; set; }

    [JsonPropertyName("figiComposite")]
    public string FigiComposite { get; set; }

    [JsonPropertyName("isin")]
    public string Isin { get; set; }

    [JsonPropertyName("mic")]
    public string Mic { get; set; }

    [JsonPropertyName("shareClassFIGI")]
    public string ShareClassFigi { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("symbol2")]
    public string Symbol2 { get; set; }
}