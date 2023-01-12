using Cheesebyte.HoldMyCrypto.Importers.Interfaces;

namespace Cheesebyte.HoldMyCrypto.Importers.Utils;

/// <summary>
/// Abstract base object for mapping functionality.
/// </summary>
public abstract class AbstractBaseMapper
{
    protected IAssetRangeImporter RangeImporter { get; }

    protected AbstractBaseMapper(IAssetRangeImporter assetRangeImporter)
    {
        RangeImporter = assetRangeImporter;
    }

    /// <summary>
    /// Returns true if the implementation for this mapper supports mapping
    /// between the two requested assets.
    /// </summary>
    /// <param name="baseAsset">Asset to convert from, such as 'BTC'.</param>
    /// <param name="quoteAsset">Asset to convert to, such as 'USDT'.</param>
    /// <returns>
    /// Whether performing actions on the requested pair is supported.
    /// </returns>
    public abstract bool IsSymbolPairSupported(string baseAsset, string quoteAsset);
    
    /// <summary>
    /// Returns the specific market symbol between a pair of two assets.
    /// </summary>
    /// <param name="baseAsset">Asset to convert from, such as 'BTC'.</param>
    /// <param name="quoteAsset">Asset to convert to, such as 'USDT'.</param>
    /// <returns>
    /// Market symbol according to the implemented format (e.g. 'BTCUSDT',
    /// 'BTC-USDT', 'BTC_USDT', depending on what the source format requires).
    /// </returns>
    public abstract string GenerateMarketSymbol(string baseAsset, string quoteAsset);
}