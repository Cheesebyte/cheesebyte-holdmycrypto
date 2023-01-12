using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Enums;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;
using Refit;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.RestApi;

/// <summary>
/// Interface for a Refit client with Bybit Wallet API endpoints.
/// </summary>
public interface IBybitWalletApi
{
    /// <summary>
    /// <para>
    /// Get wallet fund records. Returns incomplete information for transfers
    /// involving the derivatives wallet. Use the account asset API for
    /// creating and querying internal transfers (currently not implemented
    /// as part of this C# wrapper API).
    /// </para>
    /// <para>
    /// Use this for retrieval of withdrawal / deposit / profit-or-loss records
    /// from before or on 2021-07-15. Changes after that won't be returned due
    /// to changes in the Bybit APIs. Use <see cref="IBybitAssetApi"/> to
    /// retrieve withdrawals or deposits for more recent dates.
    /// </para>
    /// <para>
    /// https://bybit-exchange.github.io/docs/futuresV2/inverse/?console#t-walletrecords
    /// </para>
    /// </summary>
    /// <param name="coin">
    /// Currency alias. Returns all wallet balances if it's empty. Multiple
    /// coins allowed, separated by comma (e.g. 'USDT,USDC').
    /// </param>
    /// <param name="startTime">
    /// Start time in milliseconds. Default value: 30 days before the current time.
    /// </param>
    /// <param name="endTime">
    /// End time in milliseconds. Default value: current time.
    /// </param>
    /// <param name="walletFundType">
    /// Wallet funding as defined by <see cref="BybitWalletFundType"/>.
    /// </param>
    /// <returns></returns>
    [Get("/v2/private/wallet/fund/records")]
    Task<V2BybitListResponse<V2BybitWalletFundDetail>> GetWalletFunds(
        string coin,
        [AliasAs("start_date")] string? startTime = null,
        [AliasAs("end_date")] string? endTime = null,
        [AliasAs("wallet_fund_type")] BybitWalletFundType? walletFundType = null);
}