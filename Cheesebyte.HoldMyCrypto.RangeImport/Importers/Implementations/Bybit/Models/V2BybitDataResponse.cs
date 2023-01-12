using System.Text.Json.Serialization;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

/// <summary>
/// System.Text.Json doesn't support types other than object or JsonElement.
/// Use a V2BybitDictResponse if the following structure from the Bybit V2 API
/// is required along the way.
/// [property: JsonExtensionData] Dictionary<string, JsonElement>? CoinDetails
/// </summary>
public record V2BybitDataResponse<TListType>(
    [property: JsonPropertyName("data")] ICollection<TListType>? Data
);