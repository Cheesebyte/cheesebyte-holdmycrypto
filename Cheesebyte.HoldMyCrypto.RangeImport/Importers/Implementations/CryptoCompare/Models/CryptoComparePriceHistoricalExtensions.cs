using CryptoCompare;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.CryptoCompare.Models;

/// <summary>
/// Extension methods for data from <see cref="CryptoComparePriceHistorical"/>.
/// </summary>
internal static class CryptoComparePriceHistoricalExtensions
{
    /// <summary>
    /// <para>
    /// Parses the information from <see cref="PriceHistoricalReponse"/> into a
    /// newly created instance of <see cref="CryptoComparePriceHistorical"/>.
    /// </para>
    /// <example>
    /// Example usage:
    /// <code>
    /// var response = await _client.History.HistoricalForTimestampAsync(
    ///     baseAsset, new[] { quoteAsset },
    ///     targetDateOffset, null,
    ///     CalculationType.Close, true);
    ///
    /// var historicalPrice = response.FromPriceHistoricalReponse(baseAsset, quoteAsset);
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="response">A <see cref="PriceHistoricalReponse"/>.</param>
    /// <param name="inputBase">Base asset to use as source (e.g. 'BTC').</param>
    /// <param name="inputQuote">Quote asset to use as target (e.g. 'BTC').</param>
    /// <returns>A new instance of <see cref="CryptoComparePriceHistorical"/>.</returns>
    public static CryptoComparePriceHistorical FromPriceHistoricalReponse(
        this PriceHistoricalReponse response,
        string inputBase,
        string inputQuote)
    {
        return new CryptoComparePriceHistorical(
            inputBase,
            inputQuote,
            response
                .FirstOrDefault(x => x.Key == inputBase)
                .Value
                .FirstOrDefault(x => x.Key == inputQuote)
                .Value);
    }
}