using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Models;
using Refit;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.RestApi;

/// <summary>
/// Interface for a Refit client with Bybit Account API endpoints.
/// </summary>
public interface IBybitAssetApi
{
    /// <summary>
    /// <para>
    /// Get withdrawal info for the asset named via <paramref name="coin"/>.
    /// </para>
    /// <para>
    /// https://bybit-rangeImporter.github.io/docs/account_asset/v3/#t-withdrawrecordquery
    /// </para>
    /// <remarks>
    /// The maximum difference between <paramref name="startTime"/> and
    /// <paramref name="endTime"/> is 30 days.
    /// </remarks>
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
    /// <param name="withdrawType">
    /// Default is 0: on chain.
    /// <list type="number">
    /// <item>off chain</item>
    /// <item>on and off chain</item>
    /// </list>
    /// </param>
    /// <param name="cursor">
    /// Used for pagination with values from
    /// <see cref="V3BybitListResponse{TListType}.NextPageCursor"/>.
    /// Default value is empty.
    /// </param>
    /// <param name="limit">
    /// Number of items per page. Default value is 50.
    /// </param>
    /// <returns></returns>
    [Get("/asset/v3/private/withdraw/record/query")]
    Task<V3BybitResultResponse<V3BybitWithdrawalDetail>> GetWithdrawals(
        string coin,
        long? startTime = null,
        long? endTime = null,
        string? withdrawType = "0",
        string? cursor = "",
        int? limit = 50);

    /// <summary>
    /// <para>
    /// Get deposit info for the asset named via <paramref name="coin"/>.
    /// </para>
    /// <para>
    /// https://bybit-rangeImporter.github.io/docs/account_asset/v3/#t-depositsrecordquery
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
    /// <param name="cursor">
    /// Used for pagination with values from
    /// <see cref="V3BybitListResponse{TListType}.NextPageCursor"/>.
    /// Default value is empty.
    /// </param>
    /// <param name="limit">
    /// Number of items per page. Default value is 50.
    /// </param>
    /// <returns></returns>
    [Get("/asset/v3/private/deposit/record/query")]
    Task<V3BybitResultResponse<V3BybitDepositDetail>> GetDeposits(
        string coin,
        long? startTime = null,
        long? endTime = null,
        string? cursor = "",
        int? limit = 50);
}