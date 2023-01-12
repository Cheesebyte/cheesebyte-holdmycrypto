using System.Text.Json.Serialization;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Models;

/// <summary>
/// BL3P Response Data DTO.
/// </summary>
public record Bl3pResponseData(
    [property: JsonPropertyName("page")] long Page,
    [property: JsonPropertyName("records")] long Records,
    [property: JsonPropertyName("max_page")] long MaxPage,
    [property: JsonPropertyName("transactions")] ICollection<Bl3pTransaction> Transactions
);