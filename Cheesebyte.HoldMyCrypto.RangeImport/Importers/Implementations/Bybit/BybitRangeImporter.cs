using Ardalis.GuardClauses;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Enums;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Mapping;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.RestApi;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Utils;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Importers.Utils;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Vaults.Interfaces;
using Microsoft.Extensions.Http;
using Polly;
using Refit;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit;

/// <summary>
/// Produces internal and normalised models for data from <a href="https://bybit.com">Bybit</a>.
/// <list type="bullet">
/// <item>https://bybit-exchange.github.io/docs/futuresV2/inverse/</item>
/// <item>https://bybit.com/app/user/api-management</item>
/// <item>https://testnet.bybit.com/app/user/api-management</item>
/// </list>
/// </summary>
public class BybitRangeImporter : IAssetRangeImporter
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name => "Bybit";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool HasFullHistory => false;

    private readonly IBybitAssetApi _bybitAssetApi;
    private readonly IBybitWalletApi _bybitWalletApi;
    private readonly IBybitContractApi _bybitContractApi;
    private readonly BybitMapper _mapper;

    private const string BaseUrlProduction = "https://api.bybit.com";
    private const string BaseUrlAlternative = "https://api.bytick.com";

    public BybitRangeImporter(ISecretVault secretVault)
    {
        // In a real (micro)service these handlers should be set up
        // only once to ensure that rate limits are applied in all
        // instances where the API is being used.
        var handlerPipeline = new HttpClientHandler()
            .DecorateWith(new BybitAuthHeaderHandler(secretVault))
            .DecorateWith(new PolicyHttpMessageHandler(Policy.RateLimitAsync<HttpResponseMessage>(
                15, TimeSpan.FromMinutes(2)
            )));

        var httpClient = new HttpClient(handlerPipeline);
        httpClient.BaseAddress = new Uri(BaseUrlProduction);

        _bybitAssetApi = RestService.For<IBybitAssetApi>(httpClient);
        _bybitWalletApi = RestService.For<IBybitWalletApi>(httpClient);
        _bybitContractApi = RestService.For<IBybitContractApi>(httpClient);

        _mapper = new BybitMapper(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask<IEnumerable<ExchangeTransaction>> QueryPastTransactions(
        string baseAsset,
        string quoteAsset,
        DateTime? dateStart,
        DateTime? dateEnd)
    {
        Guard.Against.Null(dateStart, message: "Missing start date");
        Guard.Against.Null(dateEnd, message: "Missing end date");
        
        var timeStart = new DateTimeOffset(dateStart.Value);
        var timeEnd = new DateTimeOffset(dateEnd.Value);

        var withdrawalResponse = await _bybitAssetApi.GetWithdrawals(
            baseAsset,
            timeStart.ToUnixTimeMilliseconds(),
            timeEnd.ToUnixTimeMilliseconds());

        var depositResponse = await _bybitAssetApi.GetDeposits(
            baseAsset,
            timeStart.ToUnixTimeMilliseconds(),
            timeEnd.ToUnixTimeMilliseconds());

        // Acquire old (e.g. daily, while your position is open) PnL records
        // up until 2021-07-15 via Bybit's V2 API. In this program, there is
        // no support for more recent data yet.
        var profitAndLossResponse = await _bybitWalletApi.GetWalletFunds(
            baseAsset,
            BybitHelpers.FormatOldApiDate(dateStart.Value),
            BybitHelpers.FormatOldApiDate(dateEnd.Value),
            walletFundType: BybitWalletFundType.RealisedPNL);

        var withdrawalTransactions = _mapper.TxFromWithdrawals(withdrawalResponse);
        var depositTransactions = _mapper.TxFromDeposits(depositResponse);
        var profitAndLossTransactions = _mapper.TxFromProfitOrLoss(profitAndLossResponse);

        // NOTE: Try to add order/trade transactions via the `IBybitAssetApi`
        //       (based on the Binance code) API implementation later.
        return withdrawalTransactions
            .Concat(depositTransactions)
            .Concat(profitAndLossTransactions);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ValueTask<IEnumerable<ExchangePrice>> QueryPastPrices(
        string baseAsset,
        string quoteAsset,
        DateTime? dateStart,
        DateTime? dateEnd)
    {
        // Let one of the other asset rangeImporter repositories handle this for
        // now. It's hard to match some of the data from Bybit, because the
        // wallet records (withdrawal, deposit, P&L) don't necessarily contain
        // anything related to a trade or order, as (for example) a deposit
        // could have been done far ahead of trading.
        var emptyPrices = Enumerable.Empty<ExchangePrice>();
        return ValueTask.FromResult(emptyPrices);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsSymbolPairSupported(string baseAsset, string quoteAsset)
    {
        return _mapper.IsSymbolPairSupported(baseAsset, quoteAsset);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Dispose()
    {
    }
}