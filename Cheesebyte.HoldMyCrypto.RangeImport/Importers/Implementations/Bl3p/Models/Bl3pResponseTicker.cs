using System.Text.Json.Serialization;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Models;

/// <summary>
/// BL3P Response Ticker DTO.
/// </summary>
/// <param name="Volume">
/// "volume":{
///   "24h":9.82160162,
///   "30d":239.16001058
/// }
/// </param>
public record Bl3pResponseTicker(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("last")] double Last,
    [property: JsonPropertyName("bid")] double Bid,
    [property: JsonPropertyName("ask")] double Ask,
    [property: JsonPropertyName("high")] long High,
    [property: JsonPropertyName("low")] double Low,
    [property:
        JsonConverter(typeof(UnixToNullableDateTimeConverter)),
        JsonPropertyName("timestamp")
    ] DateTime? Timestamp,
    [property: JsonPropertyName("volume")] Dictionary<string, double> Volume
);