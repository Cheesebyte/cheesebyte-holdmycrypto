using Binance.Net.Enums;
using Binance.Net.Objects.Models.Spot;
using Cheesebyte.HoldMyCrypto.Enums;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Binance.Mapping;

/// <summary>
/// Mapping functionality for Binance API response models.
/// </summary>
public partial class BinanceMapper
{
    /// <summary>
    /// Creates a list of internal transaction models from a Binance response.
    /// </summary>
    /// <param name="withdrawals">Binance withdrawal info.</param>
    /// <returns>A list of transformed transactions.</returns>
    public IEnumerable<ExchangeTransaction> TxFromWithdrawals(
        ICollection<BinanceWithdrawal> withdrawals)
    {
        if (!withdrawals.Any())
        {
            return Enumerable.Empty<ExchangeTransaction>();
        }

        var withdrawalTransactions = withdrawals
            .Where(x => x.Status == WithdrawalStatus.Completed)
            .SelectMany(BuildTransactions);

        return withdrawalTransactions;
    }

    private IEnumerable<ExchangeTransaction> BuildTransactions(
        BinanceWithdrawal withdrawal)
    {
        var assetAccountName = AssetUtils.ExtractAccountName(withdrawal.Asset);
        var tx = new ExchangeTransaction(
            withdrawal.ApplyTime,
            RangeImporter.Name,
            withdrawal.TransactionId,
            new ExchangeAmount(withdrawal.ApplyTime, RangeImporter.Name, withdrawal.Asset, withdrawal.Quantity),
            new ExchangeNetwork(assetAccountName, withdrawal.Address),
            ExchangeCategory.Expense,
            ExchangeTransactionType.Debit);

        var txFee = new ExchangeTransaction(
            withdrawal.ApplyTime,
            RangeImporter.Name,
            withdrawal.TransactionId,
            new ExchangeAmount(withdrawal.ApplyTime, RangeImporter.Name, withdrawal.Asset, withdrawal.TransactionFee),
            new ExchangeNetwork(assetAccountName, withdrawal.Address),
            ExchangeCategory.Fee,
            ExchangeTransactionType.Debit);

        return new[] { tx, txFee };
    }
}