using System.Text.Json.Serialization;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Models;

/// <summary>
/// BL3P Amount DTO.
/// </summary>
/// <param name="Currency">Either 'BTC' or 'EUR'.</param>
public record Bl3pAmount(
    [property: JsonNumberHandling(JsonNumberHandling.AllowReadingFromString), JsonPropertyName("value_int")] long ValueInt,
    [property: JsonConverter(typeof(DecimalConverter)), JsonPropertyName("value")] decimal Value,
    [property: JsonPropertyName("display")] string Display,
    [property: JsonPropertyName("display_short")] string DisplayShort,
    [property: JsonPropertyName("currency")] string Currency
);