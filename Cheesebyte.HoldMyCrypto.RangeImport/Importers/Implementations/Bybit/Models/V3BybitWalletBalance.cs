using System.Text.Json.Serialization;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;

public record V3BybitWalletBalance(
    [property: JsonPropertyName("coin")] string? Coin,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("equity")
    ] decimal Equity,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("walletBalance")
    ] decimal WalletBalance,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("positionMargin")
    ] decimal PositionMargin,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("availableBalance")
    ] decimal AvailableBalance,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("orderMargin")
    ] decimal OrderMargin,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("occClosingFee")
    ] decimal OccClosingFee,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("occFundingFee")
    ] decimal OccFundingFee,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("unrealisedPnl")
    ] decimal UnrealisedPnL,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("cumRealisedPnl")
    ] decimal CumulativeRealisedPnL,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("givenCash")
    ] decimal GivenCash,
    [property:
        JsonConverter(typeof(DecimalConverter)),
        JsonPropertyName("serviceCash")
    ] decimal ServiceCash
);