using Cheesebyte.HoldMyCrypto.Enums;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Services;
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
    /// <remarks>
    /// <para>
    /// Bybit uses a multi-signature cold wallet that's only processed three
    /// times a day. See <a href="https://help.bybit.com/hc/en-us/articles/900000064923-Why-did-I-not-receive-my-withdrawal-">this url</a> for more information.
    /// </para>
    /// <para>
    /// This is the reason for the missing TXIDs in transactions mapped from
    /// Bybit's API response data. They are currently added as part of the
    /// <see cref="FillTransactionIdProcessor"/> helper in a post-processing job.
    /// </para>
    /// </remarks>
    /// <param name="response">Bybit API response info.</param>
    /// <returns>A list of transformed transactions.</returns>
    public IEnumerable<ExchangeTransaction> TxFromWithdrawals(
        V3BybitResultResponse<V3BybitWithdrawalDetail> response)
    {
        // Bybit does not charge fees for both withdrawal and deposit so
        // there's no fee to extract.
        return response
            .Result
            .Items
            .Select(x =>
                new ExchangeTransaction(
                    x.CreateTime,
                    RangeImporter.Name,
                    string.Empty,
                    new ExchangeAmount(x.CreateTime, RangeImporter.Name, x.Coin, x.Amount),
                    new ExchangeNetwork(AssetUtils.ExtractAccountName(x.Coin), x.ToAddress),
                    ExchangeCategory.Expense,
                    ExchangeTransactionType.Debit));
    }
}