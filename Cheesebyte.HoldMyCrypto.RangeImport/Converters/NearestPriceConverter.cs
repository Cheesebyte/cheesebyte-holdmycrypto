using Cheesebyte.HoldMyCrypto.Converters.Interfaces;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Services.Interfaces;
using Cheesebyte.HoldMyCrypto.Utils;
using MoreLinq.Extensions;

namespace Cheesebyte.HoldMyCrypto.Converters;

/// <summary>
/// <para>
/// Performs amount conversions using the nearest price of any
/// pricing data that could be retrieved.
/// </para>
/// <inheritdoc/>
/// </summary>
public class NearestPriceConverter : IAmountConverter
{
    private readonly ILedgerService _ledgerService;
    private readonly IAssetRangeImporter _rangeImporter;

    public NearestPriceConverter(ILedgerService ledgerService, IAssetRangeImporter rangeImporter)
    {
        _ledgerService = ledgerService;
        _rangeImporter = rangeImporter;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CanConvert(ExchangeAmount amount, string targetAssetSymbol, DateTime timestamp)
    {
        // The point of this convert is to always be able to get close to an
        // actual price, even if it needs to be calculated instead of retrieved.
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ExchangeAmount Convert(ExchangeAmount amount, string targetAssetSymbol, DateTime timestamp)
    {
        var price = QueryPrice(_rangeImporter.Name, amount.MarketSymbol, targetAssetSymbol, timestamp);
        if (price == null)
        {
            return TransactionUtils.EmptyAmount(targetAssetSymbol);
        }

        return new ExchangeAmount(
            price.Timestamp,
            price.SourceName,
            targetAssetSymbol,
            amount.Quantity * price.QuoteAmount.Quantity);
    }

    private ExchangePrice? QueryPrice(
        string exchangeName,
        string symbolBase,
        string targetAssetSymbol,
        DateTime timestamp)
    {
        // Return nearest match for requested timestamp. For better
        // results, try to interpolate with closest matches around
        // this point in time. Implement later.
        var allPrices = _ledgerService.QueryRangePrices(exchangeName);
        var nearestPrice = MinByExtension
            .MinBy(allPrices, x => Math.Abs((x.Timestamp - timestamp).Ticks))
            .FirstOrDefault(x => x.BaseAsset == symbolBase);

        return nearestPrice;
    }
}