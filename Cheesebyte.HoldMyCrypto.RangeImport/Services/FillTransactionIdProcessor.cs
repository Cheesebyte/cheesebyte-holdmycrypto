using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Services.Interfaces;
using MoreLinq.Extensions;

namespace Cheesebyte.HoldMyCrypto.Services;

/// <summary>
/// <para>
/// Due to the usage of a multi-sig wallet on exchanges like Bybit, withdrawals
/// (in the case of this program and my own situation: with Binance as the receiver
/// of the deposit) are not always fully set up as received via an importer.
/// </para>
/// <para>
/// A better approach would be to add Pre and Post handlers to <see cref="IAssetRangeImporter"/>.
/// But that's not possible with the access to other transactions currently required.
/// </para>
/// <inheritdoc/>
/// </summary>
public class FillTransactionIdProcessor : IProcessor
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<ExchangeTransaction> Process(
        ICollection<ExchangeTransaction> allTransactions)
    {
        return allTransactions.Select(tx =>
        {
            // Records in C# 9 are useful to ensure immutability :)
            var txId = GetMatchingTransactionId(tx, allTransactions);
            return tx with { TransactionId = txId };
        });
    }

    private string GetMatchingTransactionId(
        ExchangeTransaction transactionToMatch,
        ICollection<ExchangeTransaction> allTransactions)
    {
        if (!string.IsNullOrWhiteSpace(transactionToMatch.TransactionId))
        {
            return transactionToMatch.TransactionId;
        }

        // Find transactions with a timestamp closest to the one we're searching a match for
        var matchingTx = FindEqualTransactions(transactionToMatch, allTransactions);

        // Empty transaction ID == withdrawals from a multi-sig wallet on Bybit to
        // Binance (deposit receiver) detected. Multi-sig transactions don't have
        // a transaction ID on Bybit.
        var foundTx = matchingTx.FirstOrDefault();
        return foundTx != null ?
            foundTx.TransactionId :
            transactionToMatch.TransactionId;
    }

    private IEnumerable<ExchangeTransaction> FindEqualTransactions(
        ExchangeTransaction transactionToMatch,
        ICollection<ExchangeTransaction> allTransactions)
    {
        // Find transactions with a timestamp closest to the one a match is required for
        var matchingTx = allTransactions
            .OrderBy(tx => tx.Timestamp)
            .Where(x =>
                x.Amount.MarketSymbol == transactionToMatch.Amount.MarketSymbol &&
                x.Network.Address     == transactionToMatch.Network.Address)
            .Where(x => !string.IsNullOrWhiteSpace(x.TransactionId))
            .Where(x => x.Type != transactionToMatch.Type);

        // Because matching transactions can only exist with an almost equal
        // timestamp it should be possible to ignore the fact that only transactions
        // from the same (min/max) timestamped batch is used to find a nearest
        // transaction timestamp in the next line.
        return MinByExtension.MinBy(
            matchingTx,
            x => Math.Abs((x.Timestamp - transactionToMatch.Timestamp).Ticks));
    }
}