using Ardalis.GuardClauses;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Mapping;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.RestApi;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Utils;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Importers.Utils;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Vaults.Interfaces;
using Microsoft.Extensions.Http;
using Polly;
using Refit;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p;

/// <summary>
/// Produces internal and normalised models for data from <a href="https://bl3p.eu">BL3P</a>.
/// <list type="bullet">
/// <item>https://github.com/BitonicNL/bl3p-api</item>
/// <item>https://bl3p.eu/api</item>
/// <item>https://bl3p.eu/security</item>
/// </list>
/// </summary>
public class Bl3PRangeImporter : IAssetRangeImporter
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name => "BL3P";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool HasFullHistory => false;

    private readonly IBl3pApi _bl3pApi;
    private readonly Bl3pMapper _mapper;
        
    // Temporary hard limit - iteration through all pages needs to be implemented
    private const int RecordsPerPage = 50;
    private const string BaseUrl = "https://api.bl3p.eu/1";

    public Bl3PRangeImporter(ISecretVault secretVault)
    {
        // In a real (micro)service these handlers should be set up
        // only once to ensure that rate limits are applied in all
        // instances where the API is being used.
        var handlerPipeline = new HttpClientHandler()
            .DecorateWith(new Bl3pAuthHeaderHandler(secretVault))
            .DecorateWith(new PolicyHttpMessageHandler(Policy.RateLimitAsync<HttpResponseMessage>(
                500, TimeSpan.FromMinutes(10)
            )));

        var httpClient = new HttpClient(handlerPipeline);
        httpClient.BaseAddress = new Uri(BaseUrl);

        _bl3pApi = RestService.For<IBl3pApi>(httpClient);
        _mapper = new Bl3pMapper(this);
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

        // No support for iterating through record pages yet
        var walletHistoryResponse = await _bl3pApi.GetWalletHistory(
            baseAsset,
            timeStart.ToUnixTimeSeconds(),
            timeEnd.ToUnixTimeSeconds(),
            RecordsPerPage);

        // No support for withdrawal or deposit transactions yet - implement by extending IBl3pApi
        var orderTransactions = _mapper.TxFromOrderDetails(walletHistoryResponse);
        return orderTransactions;
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
        return ValueTask.FromResult(Enumerable.Empty<ExchangePrice>());
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