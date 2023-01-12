using System.Text.Json.Serialization;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

public abstract record V3BybitBaseResponse(
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("retCode")
    ] int ReturnCode,
    [property: JsonPropertyName("retMsg")] string ReturnMessage
);