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
    public IEnumerable<ExchangeTransaction> TxFromDeposits(
        V3BybitResultResponse<V3BybitDepositDetail> response)
    {
        // Bybit does not charge fees for both withdrawal and deposit
        // thus there is no fee to extract.
        return response
            .Result
            .Items
            .Select(tx => new ExchangeTransaction(
                tx.SuccessAt,
                RangeImporter.Name,
                tx.TxId,
                new ExchangeAmount(tx.SuccessAt, RangeImporter.Name, tx.Coin, tx.Amount),
                new ExchangeNetwork(AssetUtils.ExtractAccountName(tx.Coin), tx.ToAddress),
                ExchangeCategory.Asset,
                ExchangeTransactionType.Credit));
    }
}