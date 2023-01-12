using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;

namespace Cheesebyte.HoldMyCrypto.Converters.Interfaces;

/// <summary>
/// Provides an interface to implement a new converter add-on for
/// <see cref="FineGrainedDetailConverterHolder"/>. An implementation
/// of this interface makes it possible to import assets from one
/// source to another.
/// </summary>
public interface IAmountConverter
{
    /// <summary>
    /// <para>
    /// Checks whether it's possible to convert <paramref name="amount"/> correctly.
    /// Usually, this means that if the <paramref name="amount"/>'s asset type
    /// does not match a type available in an implementation of
    /// <see cref="IAssetRangeImporter"/>, a conversion can't be performed via
    /// <seealso cref="Convert"/>.
    /// </para>
    /// <para>
    /// Like the HasCache method in <see cref="IItemCache"/> this method is called
    /// whenever it's required to get info asap before pursuing more complex
    /// functionality. Therefore it should (and will) not be async.
    /// </para>
    /// </summary>
    /// <param name="amount">Amount of what an asset's worth is.</param>
    /// <param name="timestamp">Point in time for this data.</param>
    /// <param name="targetAssetSymbol">
    /// Asset to convert into (e.g. 'BTC, 'USD', 'EUR').
    /// </param>
    /// <returns>
    /// True if it's possible to convert the given <paramref name="amount"/>.
    /// </returns>
    bool CanConvert(ExchangeAmount amount, string targetAssetSymbol, DateTime timestamp);

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
    ExchangeAmount Convert(ExchangeAmount amount, string targetAssetSymbol, DateTime timestamp);
}