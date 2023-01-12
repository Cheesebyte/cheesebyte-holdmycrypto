namespace Cheesebyte.HoldMyCrypto.Services.Models;

/// <summary>
/// <para>
/// In a production program, this would be scheduled every n periods
/// (e.g. every day, every 12 hours, etc) and would be called
/// automatically with the correct start and end timestamps.
/// </para>
/// <para>
/// The two input properties for the base and quote asset will
/// be combined to create a market symbol (e.g. the symbol 'BTCUSDT'
/// is what would be used by many sources of trading data).
/// </para>
/// </summary>
public class TransactionOptions
{
    /// <summary>
    /// Base asset to use for the full symbol (e.g. 'BTC').
    /// This serves as the source asset.
    /// </summary>
    public string? AssetBase { get; set; }
        
    /// <summary>
    /// Quote asset to use for the full symbol (e.g. 'USDT').
    /// This serves as the target asset.
    /// </summary>
    public string? AssetQuote { get; set; }

    /// <summary>
    /// The start time to use for working with trading data.
    /// </summary>
    public DateTime? TimeStart { get; set; }
        
    /// <summary>
    /// The end time to use for working with trading data.
    /// </summary>
    public DateTime? TimeEnd { get; set; }

    /// <summary>
    /// The symbol used to convert all other asset amounts to. Usually
    /// a stable asset like EUR or USD(T).
    /// </summary>
    public string? TargetSymbol { get; set; }
}