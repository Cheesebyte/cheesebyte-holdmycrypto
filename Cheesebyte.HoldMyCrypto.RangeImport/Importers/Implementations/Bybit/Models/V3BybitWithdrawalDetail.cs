using System.Text.Json.Serialization;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

public record V3BybitWithdrawalDetail(
    [property: JsonPropertyName("coin")] string Coin,
    [property: JsonPropertyName("chain")] string Chain,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("amount")
    ] decimal Amount,
    [property: JsonPropertyName("txID")] string TxId,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("toAddress")] string ToAddress,
    [property: JsonPropertyName("tag")] string Tag,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("withdrawFee")
    ] decimal WithdrawFee,
    [property:
        JsonConverter(typeof(UnixToNullableDateTimeConverter)),
        JsonPropertyName("createTime")
    ] DateTime CreateTime,
    [property:
        JsonConverter(typeof(UnixToNullableDateTimeConverter)),
        JsonPropertyName("updateTime")
    ] DateTime UpdateTime,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("withdrawId")
    ] int WithdrawId
);