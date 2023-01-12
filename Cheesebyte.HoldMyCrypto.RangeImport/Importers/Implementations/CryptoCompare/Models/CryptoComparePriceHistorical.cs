namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.CryptoCompare.Models;

/// <summary>
/// Historical price data from CryptoCompare.
/// </summary>
/// <param name="Base">Base asset to use as source (e.g. 'BTC').</param>
/// <param name="Quote">Quote asset to use as target (e.g. 'USDT').</param>
/// <param name="Price">
/// Historical price value for the asset denoted by
/// <see cref="Base"/> and <see cref="Quote"/>.
/// </param>
internal record CryptoComparePriceHistorical(
    string Base,
    string Quote,
    decimal Price
);