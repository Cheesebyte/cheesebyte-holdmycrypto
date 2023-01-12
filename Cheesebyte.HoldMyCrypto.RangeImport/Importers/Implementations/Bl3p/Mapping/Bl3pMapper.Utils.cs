using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Importers.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Mapping;

/// <summary>
/// Mapping functionality for BL3P API response models.
/// </summary>
public partial class Bl3pMapper : AbstractBaseMapper
{
    public Bl3pMapper(IAssetRangeImporter assetRangeImporter)
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
        var isPairSupported = (baseAsset, quoteAsset) switch
        {
            ("BTC", "EUR") => true,
            ("LTC", "EUR") => true,

            ("EUR", "BTC") => true,
            ("EUR", "LTC") => true,

            (_, _) => false
        };

        return isPairSupported;
    }
}