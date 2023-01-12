using System.Text.Json.Serialization;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

public record V2BybitWalletFundDetail(
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("id")
    ] long Id,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("user_id")
    ] long UserId,
    [property: JsonPropertyName("coin")] string Coin,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("wallet_id")
    ] long WalletId,
    [property: JsonPropertyName("type")] string Type,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("amount")
    ] decimal Amount,
    [property: JsonPropertyName("tx_id")] string TxId,
    [property: JsonPropertyName("address")] string Address,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("wallet_balance")
    ] decimal WalletBalance,
    [property: JsonPropertyName("exec_time")] DateTime ExecTime,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("cross_seq")
    ] long CrossSeq
);