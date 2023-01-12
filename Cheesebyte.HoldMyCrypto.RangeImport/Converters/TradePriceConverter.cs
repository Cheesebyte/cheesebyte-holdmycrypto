using Cheesebyte.HoldMyCrypto.Converters.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Services.Interfaces;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Converters;

/// <summary>
/// <para>
/// Performs amount conversions while using background data from the
/// trades associated with the prices (e.g. the converter interpolates
/// between actual prices from orders done by a user).
/// </para>
/// <inheritdoc/>
/// </summary>
public class TradePriceConverter : IAmountConverter
{
    private readonly ILedgerService _ledgerService;

    public TradePriceConverter(ILedgerService ledgerService)
    {
        _ledgerService = ledgerService;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CanConvert(ExchangeAmount amount, string targetAssetSymbol, DateTime timestamp)
    {
        var isSupported = _ledgerService.IsSymbolPairSupported(amount.MarketSymbol, targetAssetSymbol);
        var hasPrice = _ledgerService.HasPrice(amount.SourceName, timestamp);

        return isSupported && hasPrice;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ExchangeAmount Convert(ExchangeAmount amount, string targetAssetSymbol, DateTime timestamp)
    {
        var price = QueryPrice(amount.SourceName, amount.MarketSymbol, targetAssetSymbol, timestamp);
        return new ExchangeAmount(
            price.Timestamp,
            price.SourceName,
            targetAssetSymbol,
            amount.Quantity * price.QuoteAmount.Quantity);
    }

    private ExchangePrice QueryPrice(
        string exchangeName,
        string symbolBase,
        string targetAssetSymbol,
        DateTime timestamp)
    {
        var exactPrice = _ledgerService.QueryPrice(exchangeName, timestamp);
        return exactPrice ?? TransactionUtils.EmptyPrice(targetAssetSymbol);
    }
}