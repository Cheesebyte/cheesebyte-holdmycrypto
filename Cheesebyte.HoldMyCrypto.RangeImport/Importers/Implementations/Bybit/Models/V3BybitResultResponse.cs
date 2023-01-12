using System.Text.Json.Serialization;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

public record V3BybitResultResponse<TListType>(
    int ReturnCode,
    string ReturnMessage,
    [property: JsonPropertyName("result")] V3BybitListResponse<TListType> Result
) : V3BybitBaseResponse(ReturnCode, ReturnMessage);