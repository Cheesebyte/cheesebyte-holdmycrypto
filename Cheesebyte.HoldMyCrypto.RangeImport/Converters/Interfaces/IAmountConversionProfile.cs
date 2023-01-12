using Cheesebyte.HoldMyCrypto.Models;

namespace Cheesebyte.HoldMyCrypto.Converters.Interfaces;

/// <summary>
/// <para>
/// Allows to convert <see cref="ExchangeAmount"/> the amount of value from
/// any <see cref="ExchangeNetwork"/> to represent what it would be if it was
/// sold to the output amount at the requested time.
/// </para>
/// <para>
/// There is currently not a direct connection between amounts to be
/// converted and the currency situation on a specific rangeImporter. Results
/// would be more precise if it would be possible to couple an amount
/// of currency to a specific rangeImporter.
/// </para>
/// </summary>
public interface IAmountConversionProfile
{
    /// <summary>
    /// Converts given <paramref name="amount"/> into an amount with asset
    /// symbol <paramref name="targetAssetSymbol"/> and adjusts the amount
    /// as required by the importer rate from <see cref="IAssetRangeImporter"/>.
    /// </summary>
    /// <param name="amount">Amount of what an asset's worth is.</param>
    /// <param name="timestamp">Point in time for this data.</param>
    /// <param name="targetAssetSymbol">
    /// Asset to convert into (e.g. 'BTC, 'USD', 'EUR').
    /// <returns>
    /// The converted <paramref name="amount"/> as an <see cref="ExchangeAmount"/> object.
    /// </returns>
    ExchangeAmount Convert(ExchangeAmount amount, DateTime timestamp, string targetAssetSymbol);
}