using Binance.Net.Objects.Models.Spot;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Importers.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Binance.Mapping;

/// <summary>
/// Mapping functionality for Binance API response models.
/// </summary>
public partial class BinanceMapper : AbstractBaseMapper
{
    public BinanceMapper(IAssetRangeImporter assetRangeImporter)
        : base(assetRangeImporter) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string GenerateMarketSymbol(string baseAsset, string quoteAsset)
    {
        return $"{baseAsset}{quoteAsset}";
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool IsSymbolPairSupported(string baseAsset, string quoteAsset)
    {
        return true;
    }
        
    private decimal GetFeeForOrders(
        string symbolSide,
        IEnumerable<BinanceOrder> allOrders,
        IEnumerable<BinanceTrade> allTrades)
    {
        var orderFee = 0m;
        var assetSymbol = symbolSide.ToUpperInvariant();

        // Collect the fee for each trade from each order
        foreach (var order in allOrders)
        {
            orderFee += allTrades
                .Where(trade => trade.OrderId == order.Id)
                .Where(trade => trade.FeeAsset == assetSymbol)
                .Aggregate(0m, (acc, trade) => acc + trade.Fee);
        }

        return orderFee;
    }
}