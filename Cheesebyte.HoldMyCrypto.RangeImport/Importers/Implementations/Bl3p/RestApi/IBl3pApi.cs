using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Models;
using Refit;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.RestApi;

/// <summary>
/// Interface for a Refit client with BL3P API endpoints.
/// <list type="bullet">
/// <item>Base API: https://github.com/BitonicNL/bl3p-api/blob/master/docs/base.md</item>
/// <item>Public API: https://github.com/BitonicNL/bl3p-api/blob/master/docs/public_api/http.md</item>
/// <item>Private API: https://github.com/BitonicNL/bl3p-api/blob/master/docs/authenticated_api/http.md</item>
/// </list>
/// </summary>
public interface IBl3pApi
{
    /// <summary>
    /// Gets trade information at the current time.
    /// </summary>
    /// <param name="market">Market symbol (e.g. 'BTCEUR').</param>
    /// <returns></returns>
    [Post("/{market}/ticker")]
    Task<Bl3pResponseTicker> GetTicker(string market);

    /// <summary>
    /// Gets the transaction history from the user's wallet.
    /// </summary>
    /// <param name="currency">
    /// Currency of the wallet (e.g. 'BTC', 'EUR').
    /// </param>
    /// <param name="dateFrom">
    /// Filter the result by a Unix-timestamp. Transactions
    /// before this date will not be returned.
    /// </param>
    /// <param name="dateTo">
    /// Filter the result by a Unix-timestamp. Transactions
    /// after this date will not be returned.
    /// </param>
    /// <param name="recsPerPage">
    /// Number of records per page.
    /// </param>
    /// <returns></returns>
    [Post("/GENMKT/money/wallet/history")]
    Task<Bl3pResponse> GetWalletHistory(
        string currency,
        [AliasAs("date_from")] long dateFrom,
        [AliasAs("date_to")] long dateTo,
        [AliasAs("recs_per_page")] int recsPerPage);
}