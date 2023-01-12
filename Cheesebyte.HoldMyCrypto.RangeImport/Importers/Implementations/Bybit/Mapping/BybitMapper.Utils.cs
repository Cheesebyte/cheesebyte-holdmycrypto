using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Importers.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Mapping;

/// <summary>
/// Mapping functionality for Bybit API response models.
/// </summary>
public partial class BybitMapper : AbstractBaseMapper
{
    public BybitMapper(IAssetRangeImporter assetRangeImporter)
        : base(assetRangeImporter) { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string GenerateMarketSymbol(string baseAsset, string quoteAsset)
    {
        return $"{baseAsset}{quoteAsset}";
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool IsSymbolPairSupported(string baseAsset, string quoteAsset)
    {
        // This is what Bybit supported in 2020. Nowadays it would be better
        // to pull this data from their API instead.
        var isPairSupported = (baseAsset, quoteAsset) switch
        {
            ("BTC", "USD") => true,
            ("ETH", "USD") => true,
            ("EOS", "USD") => true,
            ("XRP", "USD") => true,
            ("BTC", "USDT") => true,

            ("USD", "BTC") => true,
            ("USD", "ETH") => true,
            ("USD", "EOS") => true,
            ("USD", "XRP") => true,
            ("USDT", "BTC") => true,

            (_, _) => false
        };

        return isPairSupported;
    }
}