using Ardalis.GuardClauses;
using Cheesebyte.HoldMyCrypto.Enums;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Models;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Mapping;

/// <summary>
/// Mapping functionality for BL3P API response models.
/// </summary>
public partial class Bl3pMapper
{
    /// <summary>
    /// Creates a list of internal transaction models from a BL3P response.
    /// </summary>
    /// <param name="response">BL3P API response info.</param>
    /// <returns>A list of transformed transactions.</returns>
    public IEnumerable<ExchangeTransaction> TxFromOrderDetails(
        Bl3pResponse response)
    {
        if (!response
            .Data
            .Transactions
            .Any())
        {
            return Enumerable.Empty<ExchangeTransaction>();
        }

        var orderTransactions = response
            .Data
            .Transactions
            .Where(x => x.Fee != null)
            .Select(BuildTransaction);

        return orderTransactions;
    }

    private ExchangeTransaction BuildTransaction(
        Bl3pTransaction transaction)
    {
        Guard.Against.Null(transaction.Date, "Transaction needs a valid date");
        Guard.Against.Null(transaction.Fee, "Transaction needs a valid fee");
        Guard.Against.Null(transaction.ContraAmount, "Transaction needs a contra amount");
        Guard.Against.NullOrEmpty(transaction.ContraAmount?.Currency, "Transaction needs a valid currency");
        
        var assetTo = transaction.ContraAmount.Currency;
        var accountName = AssetUtils.ExtractAccountName(assetTo);

        var network = new ExchangeNetwork(accountName, $"{transaction.OrderId}");
        var amount = new ExchangeAmount(
            transaction.Date.Value,
            RangeImporter.Name,
            assetTo,
            transaction.Fee.Value);

        return new ExchangeTransaction(
            transaction.Date.Value,
            RangeImporter.Name,
            transaction.Id.ToString(),
            amount,
            network,
            ExchangeCategory.Asset,
            ExchangeTransactionType.Credit);
    }
}