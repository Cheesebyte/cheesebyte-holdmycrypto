using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Extensions;

/// <summary>
/// Extension methods for <see cref="ExchangeAmount"/>.
/// </summary>
public static class ExchangeAmountExtensions
{
    /// <summary>
    /// Creates a user-readable string from an <see cref="ExchangeAmount"/>.
    /// </summary>
    /// <param name="amount">Input <see cref="ExchangeAmount"/> to use.</param>
    /// <param name="padding">Padding to add for the format specifier.</param>
    /// <returns>A string with the formatted amount (e.g. '0.00 USDT').</returns>
    public static string FormatAmount(this ExchangeAmount amount, int padding = 0)
    {
        // This would be nicer as an extendable implementation, but let's
        // not overdo it due to current use for debugging only (at the moment).
        var formatSpecifier = amount.MarketSymbol switch
        {
            "BTC" => "0.00000000",
            "USD" => "0.00",
            "USDT" => "0.00",
            "EUR" => "0.00",
            _ => "0.00000000",
        };

        var signCharacter = amount.MarketSymbol switch
        {
            "BTC" => "₿",
            "USD" => "$",
            "USDT" => "₮",
            "EUR" => "€",
            _ => " ",
        };

        var formattedAmount = amount
            .Quantity
            .ToString(formatSpecifier)
            .PadRight(padding);

        return $"{signCharacter} {formattedAmount}";
    }

    /// <summary>
    /// Sums up the total amount for an array of <see cref="ExchangeAmount"/> objects.
    /// </summary>
    /// <param name="amounts">Array of <see cref="ExchangeAmount"/> to use.</param>
    /// <param name="targetAssetSymbol">Target asset symbol.</param>
    /// <returns>
    /// A new instance of <see cref="ExchangeAmount"/> containing the total sum.
    /// </returns>
    public static ExchangeAmount Sum(
        this ICollection<ExchangeAmount> amounts,
        string targetAssetSymbol)
    {
        return !amounts.Any() ?
            TransactionUtils.EmptyAmount(targetAssetSymbol) :
            amounts.Aggregate((left, right) => left + right);
    }

    /// <summary>
    /// Sums up the total amount for an array of <see cref="ExchangeAmount"/> objects.
    /// </summary>
    /// <param name="source">
    /// Array with data to select from via <see cref="selector"/>.
    /// </param>
    /// <param name="selector">
    /// Function to select data from the <see cref="ExchangeAmount"/> parameter.
    /// </param>
    /// <param name="targetAssetSymbol">Target asset symbol.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// A new instance of <see cref="ExchangeAmount"/> containing the total sum.
    /// </returns>
    public static ExchangeAmount Sum<T>(
        this IEnumerable<T> source,
        string targetAssetSymbol,
        Func<T, ExchangeAmount> selector) =>
        Sum(source
            .Select(selector)
            .ToList(), targetAssetSymbol);
}