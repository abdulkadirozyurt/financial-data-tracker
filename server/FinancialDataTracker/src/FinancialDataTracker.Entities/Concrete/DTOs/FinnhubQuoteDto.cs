using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FinancialDataTracker.Entities.Concrete.DTOs;

public sealed record FinnhubQuoteDto
{
    [JsonPropertyName("c")]
    public decimal CurrentPrice { get; init; }

    [JsonPropertyName("o")]
    public decimal OpenPrice { get; init; }

    [JsonPropertyName("h")]
    public decimal HighPrice { get; init; }

    [JsonPropertyName("l")]
    public decimal LowPrice { get; init; }

    [JsonPropertyName("pc")]
    public decimal PreviousClosePrice { get; init; }

    [JsonPropertyName("d")]
    public decimal Change { get; init; }

    [JsonPropertyName("dp")]
    public decimal PercentChange { get; init; }

    [JsonPropertyName("t")]
    public long? Timestamp { get; init; }
}