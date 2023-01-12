using System.Text.Json.Serialization;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

public record V2BybitListResponse<TListType>(
    int ReturnCode,
    string ReturnMessage,
    [property: JsonPropertyName("result")] V2BybitDataResponse<TListType> Result
) : V2BybitBaseResponse(ReturnCode, ReturnMessage);