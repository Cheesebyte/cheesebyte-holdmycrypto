using System.Text.Json.Serialization;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Models;

public record Bl3pResponse(
    [property: JsonPropertyName("result")] string Result,
    [property: JsonPropertyName("data")] Bl3pResponseData Data
);