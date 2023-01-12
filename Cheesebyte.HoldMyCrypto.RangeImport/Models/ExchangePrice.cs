namespace Cheesebyte.HoldMyCrypto.Models;

/// <summary>
/// Basically a wrapper to hold an <see cref="ExchangeAmount"/>.
/// </summary>
/// <param name="BaseAsset">
/// Market symbol with two assets (e.g. 'USD', 'BTC', 'USDT').
/// </param>
/// <param name="QuoteAmount">
/// Value held at the time given by <see cref="BaseExchangeTimeData.Timestamp"/>.
/// </param>
public record ExchangePrice(
    DateTime Timestamp,
    string SourceName,
    string BaseAsset,
    ExchangeAmount QuoteAmount) : BaseExchangeTimeData(Timestamp, SourceName);