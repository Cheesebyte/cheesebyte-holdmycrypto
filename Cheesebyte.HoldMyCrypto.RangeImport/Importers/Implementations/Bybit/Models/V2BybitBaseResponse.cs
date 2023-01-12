using System.Text.Json.Serialization;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

public abstract record V2BybitBaseResponse(
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("ret_code")
    ] int ReturnCode,
    [property: JsonPropertyName("ret_msg")] string ReturnMessage
);