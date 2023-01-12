using System.Text.Json.Serialization;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

public record V3BybitDepositDetail(
    [property: JsonPropertyName("coin")] string Coin,
    [property: JsonPropertyName("chain")] string Chain,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("amount")
    ] decimal Amount,
    [property: JsonPropertyName("txID")] string TxId,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("status")
    ] int Status,
    [property: JsonPropertyName("toAddress")] string ToAddress,
    [property: JsonPropertyName("tag")] string Tag,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("depositFee")
    ] decimal DepositFee,
    [property:
        JsonConverter(typeof(UnixToNullableDateTimeConverter)),
        JsonPropertyName("successAt")
    ] DateTime SuccessAt,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("confirmations")
    ] int Confirmations,
    [property: JsonPropertyName("txIndex")] string TxIndex,
    [property: JsonPropertyName("blockHash")] string BlockHash
);