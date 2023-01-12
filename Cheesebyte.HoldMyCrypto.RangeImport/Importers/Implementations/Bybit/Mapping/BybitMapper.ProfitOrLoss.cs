using Ardalis.GuardClauses;
using Cheesebyte.HoldMyCrypto.Enums;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Mapping;

/// <summary>
/// Mapping functionality for Bybit API response models.
/// </summary>
public partial class BybitMapper
{
    /// <summary>
    /// Creates a list of internal transaction models from a Bybit response.
    /// </summary>
    /// <param name="response">Bybit API response info.</param>
    /// <returns>A list of transformed transactions.</returns>
    public IEnumerable<ExchangeTransaction> TxFromProfitOrLoss(
        V2BybitListResponse<V2BybitWalletFundDetail> response)
    {
        Guard.Against.Null(response.Result.Data, "Missing data in response");

        return response
            .Result
            .Data
            .Select(x =>
                new ExchangeTransaction(
                    x.ExecTime,
                    RangeImporter.Name,
                    string.Empty,
                    new ExchangeAmount(x.ExecTime, RangeImporter.Name, x.Coin, Math.Abs(x.Amount)),
                    new ExchangeNetwork(AssetUtils.ExtractAccountName(x.Coin), string.Empty),
                    x.Amount >= 0 ? ExchangeCategory.Asset : ExchangeCategory.Expense,
                    x.Amount >= 0 ? ExchangeTransactionType.Credit : ExchangeTransactionType.Debit));
    }
}