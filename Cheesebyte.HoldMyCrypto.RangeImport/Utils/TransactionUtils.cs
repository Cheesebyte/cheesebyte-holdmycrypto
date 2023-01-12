using Cheesebyte.HoldMyCrypto.Converters.Interfaces;
using Cheesebyte.HoldMyCrypto.Enums;
using Cheesebyte.HoldMyCrypto.Extensions;
using Cheesebyte.HoldMyCrypto.Models;

namespace Cheesebyte.HoldMyCrypto.Utils;

/// <summary>
/// Utilities to help management of <see cref="ExchangeTransaction"/> objects.
/// </summary>
public static class TransactionUtils
{
    /// <summary>
    /// Helper to create an empty <see cref="ExchangeAmount"/>. Useful in
    /// LINQ queries and other areas where a 'base' amount is required.
    /// </summary>
    /// <param name="marketSymbol">Target asset (e.g. 'BTC', 'USDT').</param>
    /// <returns>
    /// An zero version of <see cref="ExchangePrice"/> with
    /// <see cref="marketSymbol"/> and no mention of a source.
    /// </returns>
    public static ExchangeAmount EmptyAmount(string marketSymbol) =>
        new(DateTime.UtcNow,
            string.Empty,
            marketSymbol,
            0m);

    /// <summary>
    /// Helper to create an empty <see cref="ExchangePrice"/>. Useful in
    /// LINQ queries and other areas where a 'base' amount is required.
    /// </summary>
    /// <param name="marketSymbol">Target asset (e.g. 'BTC', 'USDT').</param>
    /// <returns>
    /// An zero version of <see cref="ExchangePrice"/> with
    /// <see cref="marketSymbol"/> and no mention of a source.
    /// </returns>
    public static ExchangePrice EmptyPrice(string marketSymbol) =>
        new(DateTime.UtcNow,
            string.Empty,
            marketSymbol,
            EmptyAmount(marketSymbol));

    /// <summary>
    /// Calculates the total sum for assets denoted by
    /// <paramref name="targetAssetSymbol"/> in category <paramref name="category"/>.
    /// </summary>
    /// <param name="transactions">List of transactions.</param>
    /// <param name="converter">Conversion profile.</param>
    /// <param name="timestamp">Point in time to use.</param>
    /// <param name="targetAssetSymbol">Target asset symbol.</param>
    /// <param name="category">Transaction category to use.</param>
    /// <returns>
    /// A new <see cref="ExchangeAmount"/> with the total value of all input amounts.
    /// </returns>
    public static ExchangeAmount CalculateTotalAmount(
        IEnumerable<ExchangeTransaction> transactions,
        IAmountConversionProfile converter,
        DateTime timestamp,
        string targetAssetSymbol,
        ExchangeCategory category)
    {
        var emptyAmount = EmptyAmount(targetAssetSymbol);
        var totalAmountInTargetAsset = transactions
            .Where(tx => tx.Category == category)
            .Select(tx => converter.Convert(tx.Amount, timestamp, targetAssetSymbol))
            .DefaultIfEmpty(emptyAmount)
            .ToList()
            .Sum(targetAssetSymbol);

        return totalAmountInTargetAsset;
    }

    private static IEnumerable<ExchangeTransaction> GetLinkedTransactions(
        ExchangeTransaction linkTransaction,
        IEnumerable<ExchangeTransaction> transactions)
    {
        var allLinkedTransactions = transactions
            // Skip invalid transaction IDs
            .Where(x => !string.IsNullOrWhiteSpace(x.TransactionId))
            .Where(x => x.TransactionId == linkTransaction.TransactionId)
            .Where(x => x.Type != ExchangeTransactionType.Transfer);

        return allLinkedTransactions;
    }
}