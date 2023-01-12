using Cheesebyte.HoldMyCrypto.Caching.Interfaces;
using Cheesebyte.HoldMyCrypto.Converters.Interfaces;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Services.Models;

namespace Cheesebyte.HoldMyCrypto.Services.Interfaces;

/// <summary>
/// Schedules loading and processing <see cref="ExchangeTransaction"/>
/// from one or more importers (<see cref="IAssetRangeImporter"/>).
/// </summary>
public interface ILedgerService
{
    /// <summary>
    /// Standard conversion method that includes all <see cref="IAssetRangeImporter"/>
    /// importers as a possible source for conversion data.
    /// </summary>
    IAmountConversionProfile ConversionProfile { get; }

    /// <summary>
    /// Loads any transaction found in included importers to <see cref="IItemCache{TItem}"/>.
    /// All new transactions can be found in that cache once they're
    /// fully loaded and processed.
    /// </summary>
    Task<TransactionUpdateResult> UpdateAllFromSource();

    /// <summary>
    /// Retrieves known transactions from each importer currently loaded.
    /// </summary>
    IEnumerable<ExchangeTransaction> QueryTransactions();

    /// <summary>
    /// Retrieves known prices from each importer currently loaded.
    /// </summary>
    /// <returns>A list of prices that match given input.</returns>
    IEnumerable<ExchangePrice> QueryRangePrices(string networkName);

    /// <summary>
    /// Quickly informs caller whether pricing data is available for
    /// the given data to match.
    /// </summary>
    /// <returns>True if the price matching the input data is available.</returns>
    bool HasPrice(string networkName, DateTime timestamp);

    /// <summary>
    /// Retrieves known prices from any importer currently loaded.
    /// </summary>
    /// <returns>An <see cref="ExchangePrice"/> that matches given input.</returns>
    ExchangePrice? QueryPrice(string networkName, DateTime timestamp);

    /// <summary>
    /// Quickly informs caller whether a pair can be used or not.
    /// </summary>
    /// <returns>True if the symbol pair is actually supported.</returns>
    bool IsSymbolPairSupported(string baseAsset, string quoteAsset);
}