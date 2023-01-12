using Cheesebyte.HoldMyCrypto.Converters.Interfaces;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;

namespace Cheesebyte.HoldMyCrypto.Converters;

/// <summary>
/// <para>
/// Performs amount conversions with realtime pricing data instead
/// of via data backed by data from daily or trading prices.
/// </para>
/// <inheritdoc/>
/// </summary>
public class RealtimePriceConverter : IAmountConverter
{
    private readonly IAssetRangeImporter _rangeImporter;

    public RealtimePriceConverter(IAssetRangeImporter rangeImporter)
    {
        _rangeImporter = rangeImporter;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CanConvert(ExchangeAmount amount, string targetAssetSymbol, DateTime timestamp)
    {
        // See comment in Convert method
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ExchangeAmount Convert(
        ExchangeAmount amount,
        string targetAssetSymbol,
        DateTime timestamp)
    {
        // Currently not supported because it was removed from an
        // older version of this code. Should re-implement.
        return new ExchangeAmount(
            amount.Timestamp,
            amount.SourceName,
            amount.MarketSymbol,
            0m);
    }
}