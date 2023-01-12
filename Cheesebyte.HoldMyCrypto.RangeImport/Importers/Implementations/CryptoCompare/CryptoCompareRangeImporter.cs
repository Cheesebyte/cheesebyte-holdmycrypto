using Ardalis.GuardClauses;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Vaults.Interfaces;
using CryptoCompare;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.CryptoCompare;

/// <summary>
/// Produces internal and normalised models for data from <a href="https://cryptocompare.com/">CryptoCompare</a>.
/// <list type="bullet">
/// <item>https://min-api.cryptocompare.com/documentation</item>
/// <item>https://www.cryptocompare.com/cryptopian/api-keys</item>
/// </list>
/// </summary>
public class CryptoCompareRangeImporter : IAssetRangeImporter
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name => "CryptoCompare";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool HasFullHistory => true;

    private readonly ICryptoCompareClient _client;

    private const int ThrottleDelayMs = 40;
    private const string TotalDaysError = "Cannot query more than 2000 days between start and end date.";

    public CryptoCompareRangeImporter(ISecretVault secretVault)
    {
        var apiKey = secretVault.GetSecureKey<string>($"{Name}/ApiKey") ?? string.Empty;
        Guard.Against.NullOrEmpty(apiKey, "Missing CryptoCompare API key");

        _client = new CryptoCompareClient(apiKey, ThrottleDelayMs);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ValueTask<IEnumerable<ExchangeTransaction>> QueryPastTransactions(
        string baseAsset,
        string quoteAsset,
        DateTime? dateStart,
        DateTime? dateEnd)
    {
        var emptyTransactionList = Enumerable.Empty<ExchangeTransaction>();
        return ValueTask.FromResult(emptyTransactionList);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask<IEnumerable<ExchangePrice>> QueryPastPrices(
        string baseAsset,
        string quoteAsset,
        DateTime? dateStart,
        DateTime? dateEnd)
    {
        var start = dateStart ?? DateTime.UtcNow;
        var end = dateEnd ?? DateTime.UtcNow;
        var totalDays = (int)Math.Floor((end - start).TotalDays);
        
        // CryptoCompare has a default limit of 2000
        // See: https://min-api.cryptocompare.com/documentation?key=Historical&cat=dataHistoday
        Guard.Against.AgainstExpression(x => x is <= 0 and <= 2000, totalDays, TotalDaysError);
        
        // HistoricalForTimestampAsync would be used for a single item, but this
        // is better because it allows us to request this data in larger batches
        // and request less from the API (costs more money).
        var historicalDailyPriceResponse = await _client.History.DailyAsync(
            baseAsset, quoteAsset,
            totalDays, null, dateEnd,
            null, null, true);

        var pastTimestamps = historicalDailyPriceResponse
            .Data
            .Select(candleData => new ExchangePrice(
                candleData.Time.Date,
                Name,
                baseAsset,
                new ExchangeAmount(
                    candleData.Time.Date,
                    Name,
                    quoteAsset,
                    candleData.Close)));

        return pastTimestamps;
    }
    
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsSymbolPairSupported(string baseAsset, string quoteAsset)
    {
        // Assume true for now, but in the future it might be useful to
        // actually check what CryptoCompare supports. Can be done
        // via `_client.Coins.ListAsync();`.
        return true;
    }
    
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Dispose()
    {
        _client.Dispose();
    }
}