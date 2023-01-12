using Cheesebyte.HoldMyCrypto.Converters.Interfaces;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Services.Interfaces;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Converters;

/// <summary>
/// <para>
/// Performs amount conversions with the finest pricing details available.
/// </para>
/// <inheritdoc/>
/// </summary>
public class FineGrainedDetailConverterHolder : IAmountConversionProfile
{
    private readonly ICollection<IAmountConverter> _addOns;

    public FineGrainedDetailConverterHolder(
        IEnumerable<IAssetRangeImporter> exchanges,
        ILedgerService transactionService)
    {
        _addOns = new List<IAmountConverter>
        {
            new TradePriceConverter(transactionService)
        };

        // Pick non-paid service for realtime rangeImporter of currency.
        // Could expand this to become configurable in the future.
        var exchangeWithFullHistory = exchanges.FirstOrDefault(x => x.HasFullHistory);
        if (exchangeWithFullHistory != null)
        {
            _addOns.Add(new NearestPriceConverter(transactionService, exchangeWithFullHistory));
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ExchangeAmount Convert(ExchangeAmount amount, DateTime timestamp, string targetAssetSymbol)
    {
        if (amount.MarketSymbol == targetAssetSymbol)
        {
            // Should never reach this point. If it happens, it's a programming error.
            var errorHint = "do you have enough data or realtime exchanges to support this conversion?";
            var errorMessage = $"Cannot convert from '{amount.MarketSymbol}' to '{targetAssetSymbol}' - {errorHint}";

            throw new InvalidOperationException(errorMessage);
        }

        // Get the first add-on that can convert between different
        // asset types and isn't the same as the input asset type either.
        var addOn = _addOns
            .FirstOrDefault(x =>
                x.CanConvert(amount, targetAssetSymbol, timestamp));

        return addOn != null ?
            addOn.Convert(amount, targetAssetSymbol, timestamp) :
            TransactionUtils.EmptyAmount(targetAssetSymbol);
    }
}