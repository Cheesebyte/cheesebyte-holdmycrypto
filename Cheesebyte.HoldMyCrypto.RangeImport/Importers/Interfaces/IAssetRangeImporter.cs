using Cheesebyte.HoldMyCrypto.Models;

namespace Cheesebyte.HoldMyCrypto.Importers.Interfaces;

/// <summary>
/// <para>
/// This is like a wrapper for (example) exchanges, but not just that. Importers
/// exist to provide an interface to info outside of exchanges, importers based
/// on data sets (e.g. Big Mac index), or importers based on markets across
/// a wide spectrum of varying items (e.g. ordinary currencies vs crypto).
/// </para>
/// <remarks>
/// When adding functionality here, please ensure too focus less on specific
/// functionality for single results. It's better to return a collection of
/// queried results, than a single result.
/// </remarks>
/// </summary>
public interface IAssetRangeImporter: IDisposable
{
    /// <summary>
    /// Name to describe the underlying asset rangeImporter. Could be anything
    /// from "Binance", to "N26 Bank" or your local bank.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Normal exchanges often don't have all historic pricing (or other)
    /// data. This property indicates that the implementation knows how to
    /// reach all data, including data that might be missing when dealing
    /// with order and trade info from 'normal' exchanges.
    /// </summary>
    bool HasFullHistory { get; }

    /// <summary>
    /// <para>
    /// Queries all the internal orders from the implemented importer and
    /// transforms them to native <see cref="ExchangeTransaction"/> objects.
    /// The same concept is applied to deposits and withdrawals.
    /// </para>
    /// <para>
    /// Transactions are balanced if possible, with credit and debit
    /// transactions to indicate in or outgoing assets.
    /// </para>
    /// </summary>
    /// <param name="baseAsset">Base asset name (e.g. 'BTC', 'USDT', 'EUR').</param>
    /// <param name="quoteAsset">Quote asset name (e.g. 'BTC', 'USDT', 'EUR').</param>
    /// <returns></returns>
    ValueTask<IEnumerable<ExchangeTransaction>> QueryPastTransactions(
        string baseAsset,
        string quoteAsset,
        DateTime? dateStart,
        DateTime? dateEnd);

    /// <summary>
    /// Queries quote prices between the requested dates.
    /// </summary>
    /// <param name="baseAsset">Base asset name (e.g. 'BTC', 'USDT', 'EUR').</param>
    /// <param name="quoteAsset">Quote asset name (e.g. 'BTC', 'USDT', 'EUR').</param>
    /// <returns></returns>
    ValueTask<IEnumerable<ExchangePrice>> QueryPastPrices(
        string baseAsset,
        string quoteAsset,
        DateTime? dateStart,
        DateTime? dateEnd);

    /// <summary>
    /// Indicates whether this <see cref="IAssetRangeImporter"/> supports buying
    /// or selling of assets with given asset types (<paramref name="baseAsset"/>
    /// and <paramref name="quoteAsset"/>).
    /// </summary>
    /// <param name="baseAsset">Base asset name (e.g. 'BTC', 'USDT', 'EUR').</param>
    /// <param name="quoteAsset">Quote asset name (e.g. 'BTC', 'USDT', 'EUR').</param>
    /// <returns></returns>
    bool IsSymbolPairSupported(string baseAsset, string quoteAsset);
}