namespace Cheesebyte.HoldMyCrypto.Utils;

/// <summary>
/// Utilities to help management of financial asset metadata.
/// </summary>
public static class AssetUtils
{
    /// <summary>
    /// Creates a human readable name from <paramref name="assetSymbol"/>.
    /// </summary>
    /// <param name="assetSymbol">Single asset symbol (e.g. 'BTC').</param>
    /// <returns>A network name (e.g. 'Bitcoin').</returns>
    public static string ExtractAccountName(string? assetSymbol)
    {
        return assetSymbol switch
        {
            "BTC" => "Bitcoin",
            "ETH" => "Ethereum",
            "USDT" => "Tether US",
            "EUR" => "Euro",
            _ => $"Unknown asset '{assetSymbol}'"
        };
    }

    /// <summary>
    /// Separate the base and quote strings for <paramref name="symbol"/>.
    /// </summary>
    /// <param name="symbol">Input symbol (e.g. 'BTCUSDT').</param>
    /// <returns>
    /// A tuple with the base and quote string as separated values.
    /// </returns>
    public static (string Base, string Quote) GetBaseQuoteFromSymbol(string symbol)
    {
        // Should rather support any base symbol by reading
        // the input symbol string (e.g. 'BTCUSDT' == 'BTC')
        // but requires more information to make that work.
        return ("BTC", symbol.Replace("BTC", string.Empty));
    }
}