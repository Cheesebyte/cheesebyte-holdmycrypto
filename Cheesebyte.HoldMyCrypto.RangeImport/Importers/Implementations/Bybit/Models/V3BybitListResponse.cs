using System.Text.Json.Serialization;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

/// <summary>
/// It's odd to have both <see cref="Result"/> and <see cref="Rows"/> with
/// the same types of data. The only thing that can be done here is to
/// handle it as gracefully as possible.
/// </summary>
public record V3BybitListResponse<TListType>(
    [property: JsonPropertyName("list")] ICollection<TListType> Items,
    [property: JsonPropertyName("rows")] ICollection<TListType> Rows,
    [property: JsonPropertyName("nextPageCursor")] string? NextPageCursor
);