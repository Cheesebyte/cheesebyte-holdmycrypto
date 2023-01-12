using System.Text.Json.Serialization;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

public record V3BybitPastPosition(
    [property: JsonPropertyName("symbol")] string Symbol,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("execFee")
    ] decimal ExecFee,
    [property: JsonPropertyName("execId")] string ExecId,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("execPrice")
    ] decimal ExecPrice,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("execQty")
    ] long ExecQty,
    [property: JsonPropertyName("execType")] string ExecType,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("execValue")
    ] decimal ExecValue,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("feeRate")
    ] decimal FeeRate,
    [property: JsonPropertyName("lastLiquidityInd")] string LastLiquidityInd,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("leavesQty")
    ] long LeavesQty,
    [property: JsonPropertyName("orderId")] string OrderId,
    [property: JsonPropertyName("orderLinkId")] string OrderLinkId,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("orderPrice")
    ] decimal OrderPrice,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("orderQty")
    ] long OrderQty,
    [property: JsonPropertyName("orderType")] string OrderType,
    [property: JsonPropertyName("stopOrderType")] string StopOrderType,
    [property: JsonPropertyName("side")] string Side,
    [property: JsonPropertyName("execTime")] string ExecTime,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("closedSize")
    ] long ClosedSize,
    [property: JsonPropertyName("iv")] string Iv,
    [property: JsonPropertyName("blockTradeId")] string BlockTradeId,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("markPrice")
    ] decimal MarkPrice,
    [property: JsonPropertyName("markIv")] string MarkIv,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("underlyingPrice")
    ] decimal UnderlyingPrice,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("indexPrice")
    ] decimal IndexPrice
);