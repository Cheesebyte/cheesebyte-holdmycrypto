using System.Text.Json.Serialization;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

public record V3BybitOrder(
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("side")] string Side,
    [property: JsonPropertyName("orderType")] string OrderType,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("price")
    ] decimal Price,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("qty")
    ] long Qty,
    [property: JsonPropertyName("reduceOnly")] bool ReduceOnly,
    [property: JsonPropertyName("timeInForce")] string TimeInForce,
    [property: JsonPropertyName("orderStatus")] string OrderStatus,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("leavesQty")
    ] long LeavesQty,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("leavesValue")
    ] long LeavesValue,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("cumExecQty")
    ] long CumExecQty,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("cumExecValue")
    ] decimal CumExecValue,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("cumExecFee")
    ] decimal CumExecFee,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("lastPriceOnCreated")
    ] decimal LastPriceOnCreated,
    [property: JsonPropertyName("rejectReason")] string RejectReason,
    [property: JsonPropertyName("orderLinkId")] string OrderLinkId,
    [property:
        JsonConverter(typeof(UnixToNullableDateTimeConverter)),
        JsonPropertyName("createdTime")
    ] DateTime? CreatedTime,
    [property:
        JsonConverter(typeof(UnixToNullableDateTimeConverter)),
        JsonPropertyName("updatedTime")
    ] DateTime? UpdatedTime,
    [property: JsonPropertyName("orderId")] string OrderId,
    [property: JsonPropertyName("stopOrderType")] string StopOrderType,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("takeProfit")
    ] decimal TakeProfit,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("stopLoss")
    ] decimal StopLoss,
    [property: JsonPropertyName("tpTriggerBy")] string TpTriggerBy,
    [property: JsonPropertyName("slTriggerBy")] string SlTriggerBy,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("triggerPrice")
    ] decimal TriggerPrice,
    [property: JsonPropertyName("closeOnTrigger")] bool CloseOnTrigger,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("triggerDirection")
    ] long TriggerDirection,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("positionIdx")
    ] long PositionIdx
);