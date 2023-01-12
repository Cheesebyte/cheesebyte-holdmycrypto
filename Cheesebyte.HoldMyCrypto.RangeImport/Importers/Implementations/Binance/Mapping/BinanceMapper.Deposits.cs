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
    /// <param name="deposits">Binance deposit info.</param>
    /// <returns>A list of transformed transactions.</returns>
    public IEnumerable<ExchangeTransaction> TxFromDeposits(ICollection<BinanceDeposit> deposits)
    {
        if (!deposits.Any())
        {
            return Enumerable.Empty<ExchangeTransaction>();
        }

        var depositTransactions = deposits
            .Where(x => x.Status == DepositStatus.Success)
            .Select(BuildTransaction);

        return depositTransactions;
    }

    private ExchangeTransaction BuildTransaction(BinanceDeposit deposit)
    {
        var assetAccountName = AssetUtils.ExtractAccountName(deposit.Asset);
        return new ExchangeTransaction(
            deposit.InsertTime,
            RangeImporter.Name,
            deposit.Id,
            new ExchangeAmount(deposit.InsertTime, RangeImporter.Name, deposit.Asset, deposit.Quantity),
            new ExchangeNetwork(assetAccountName, deposit.Address),
            ExchangeCategory.Asset,
            ExchangeTransactionType.Credit);
    }
}