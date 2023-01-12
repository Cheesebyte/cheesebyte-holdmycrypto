using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;
using Refit;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.RestApi;

/// <summary>
/// Interface for a Refit client with Bybit (Derivative) Contract API endpoints.
/// </summary>
public interface IBybitContractApi
{
    /// <summary>
    /// <para>
    /// Get wallet balance info for the asset named via <paramref name="coin"/>.
    /// </para>
    /// <para>
    /// https://bybit-rangeImporter.github.io/docs/derivativesV3/contract/#t-balance
    /// </para>
    /// </summary>
    /// <param name="coin">
    /// Currency alias. Returns all wallet balances if it's null or empty.
    /// Multiple coins allowed, separated by comma (e.g. 'USDT,USDC').
    /// </param>
    /// <returns></returns>
    [Get("/contract/v3/private/account/wallet/balance")]
    Task<V3BybitResultResponse<V3BybitWalletBalance>> GetWalletBalance(string coin = "");

    /// <summary>
    /// <para>
    /// Access user's order list. As order creation/cancellation is
    /// asynchronous, the data returned from the interface may be delayed.
    /// </para>
    /// <para>
    /// https://bybit-rangeImporter.github.io/docs/derivativesV3/contract/#t-contract_getorder
    /// </para>
    /// </summary>
    /// <param name="symbol">
    /// Market symbol as a combination of one base and one quote asset (e.g. 'BTCUSDT').
    /// </param>
    /// <returns></returns>
    [Get("/contract/v3/private/order/list")]
    Task<V3BybitResultResponse<V3BybitOrder>> GetOrderList(string symbol);

    /// <summary>
    /// <para>
    /// Get user's trading records. The results are ordered in descending
    /// order (the first item is the latest). Returns records up to 2 years old.
    /// </para>
    /// <para>
    /// https://bybit-rangeImporter.github.io/docs/derivativesV3/contract/#t-usertraderecords
    /// </para>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [Get("/contract/v3/private/execution/list")]
    Task<V3BybitResultResponse<V3BybitPastPosition>> GetPositionList(string symbol);
}