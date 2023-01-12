using Cheesebyte.HoldMyCrypto.Converters;
using Cheesebyte.HoldMyCrypto.Extensions;

namespace Cheesebyte.HoldMyCrypto.Models;

/// <summary>
/// Amount (as in currency) specifier and its quantity.
/// </summary>
/// <param name="MarketSymbol">
/// Market symbol with two assets (e.g. 'USD', 'BTC', 'USDT').
/// </param>
/// <param name="Quantity">
/// The number of units to be held
/// for the <see cref="MarketSymbol"/> specifier.
/// </param>
public record ExchangeAmount(
    DateTime Timestamp,
    string SourceName,
    string MarketSymbol,
    decimal Quantity) : BaseExchangeTimeData(Timestamp, SourceName)
{
    /// <summary>
    /// Creates a new instance with <paramref name="right"/> added
    /// to <paramref name="left"/>.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static ExchangeAmount operator + (ExchangeAmount left, ExchangeAmount right)
    {
        ThrowErrorOnInvalidType(left, right);
        return left with { Quantity = left.Quantity + right.Quantity };
    }

    /// <summary>
    /// Creates a new instance with <paramref name="right"/> subtracted
    /// from <paramref name="left"/>.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static ExchangeAmount operator - (ExchangeAmount left, ExchangeAmount right)
    {
        ThrowErrorOnInvalidType(left, right);
        return new ExchangeAmount(
            left.Timestamp,
            left.SourceName,
            left.MarketSymbol,
            left.Quantity - right.Quantity)
        {
            SourceName = left.SourceName,
        };
    }

    /// <summary>
    /// Creates a new instance with <paramref name="right"/> multiplied
    /// with <paramref name="left"/>.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static ExchangeAmount operator * (ExchangeAmount left, ExchangeAmount right)
    {
        ThrowErrorOnInvalidType(left, right);
        return new ExchangeAmount(
            left.Timestamp,
            left.SourceName,
            left.MarketSymbol,
            left.Quantity + right.Quantity)
        {
            SourceName = left.SourceName,
        };
    }

    private static void ThrowErrorOnInvalidType(ExchangeAmount left, ExchangeAmount right)
    {
        var symbolLeft = left.MarketSymbol;
        var symbolRight = right.MarketSymbol;

        if (symbolLeft.Equals(symbolRight))
        {
            return;
        }

        // Provide a hint on a way to make this error disappear
        var profile = nameof(FineGrainedDetailConverterHolder);
        var hint = $"Please use {profile} to convert into correct specifier.";

        throw new ArgumentException($"[{symbolLeft} - {symbolRight}]: {hint}");
    }

    /// <summary>
    /// Overrides the built-in toString functionality to display both the
    /// <see cref="MarketSymbol"/> and <see cref="Quantity"/> in a user readable
    /// way. Includes format specifiers for known types such as USD(T) and BTC.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.FormatAmount(1);
    }
}