using Cheesebyte.HoldMyCrypto.Caching.Interfaces;
using Cheesebyte.HoldMyCrypto.Converters;
using Cheesebyte.HoldMyCrypto.Converters.Interfaces;
using Cheesebyte.HoldMyCrypto.Extensions;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Services.Interfaces;
using Cheesebyte.HoldMyCrypto.Services.Models;
using FluentValidation;
using Microsoft.Extensions.Options;
using MoreLinq.Extensions;

namespace Cheesebyte.HoldMyCrypto.Services;

/// <summary>
/// <para>
/// Supports retrieval of transactional information with automatic usage of caching.
/// </para>
/// <inheritdoc/>
/// </summary>
public class MainLedgerService : ILedgerService
{
    public IAmountConversionProfile ConversionProfile { get; }

    private readonly IItemCache<ExchangeTransaction> _transactionCache;
    private readonly IItemCache<ExchangePrice> _pricingCache;

    private readonly IProcessor _processor;
    private readonly ICollection<IAssetRangeImporter> _exchanges;
    private readonly TransactionOptions _options;

    public MainLedgerService(
        IEnumerable<IAssetRangeImporter> exchanges,
        IItemCache<ExchangeTransaction> transactionCache,
        IItemCache<ExchangePrice> pricingCache,
        IProcessor processor,
        IOptions<TransactionOptions> options,
        IValidator<TransactionOptions> validator)
    {
        validator.ValidateAndThrow(options.Value);
        
        _transactionCache = transactionCache;
        _pricingCache = pricingCache;

        _processor = processor;
        _exchanges = exchanges.ToList();
        _options = options.Value;

        ConversionProfile = new FineGrainedDetailConverterHolder(_exchanges, this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<ExchangeTransaction> QueryTransactions()
    {
        // Only works via cache
        return _transactionCache.LoadRange(string.Empty);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<ExchangePrice> QueryRangePrices(string networkName)
    {
        // Only works via cache
        var results = _pricingCache
            .LoadRange(networkName)
            .OrderBy(pr => pr.Timestamp)
            .GroupBy(pr => pr.Timestamp.Date)
            .Select(pr => pr.ToList());

        var total = results.SelectMany(x => x.ToList());
        return total;
    }
    
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool HasPrice(string networkName, DateTime timestamp)
    {
        var preferredExchange = GetPreferredPriceNetwork(networkName, timestamp);
        return !string.IsNullOrWhiteSpace(preferredExchange);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ExchangePrice? QueryPrice(string networkName, DateTime timestamp)
    {
        // Only works via cache - try to find a single exact match for the requested timestamp
        var preferredNetwork = GetPreferredPriceNetwork(networkName, timestamp);
        if (string.IsNullOrWhiteSpace(preferredNetwork))
        {
            return null;
        }

        var cachedExchangePrice = _pricingCache.Load(preferredNetwork, timestamp);
        return cachedExchangePrice;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<TransactionUpdateResult> UpdateAllFromSource()
    {
        // 1) Transactions
        var transactions = await BuildTransactionsFromImporter();
        transactions.ForEach(_transactionCache.Save);

        // 2) Prices
        var pairCombinations = BuildPairCombinations(transactions);
        foreach (var pair in pairCombinations)
        {
            foreach (var singleExchange in _exchanges)
            {
                var prices = await BuildPricesFromImporter(
                    singleExchange,
                    pair.Base,
                    pair.Quote);

                prices.ForEach(_pricingCache.Save);
            }
        }

        // 3) Results
        return new TransactionUpdateResult
        {
            RetrievedCount = transactions.Count,
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsSymbolPairSupported(string baseAsset, string quoteAsset) =>
        _exchanges.Any(x =>
            x.IsSymbolPairSupported(baseAsset, quoteAsset));
    
    private string GetPreferredPriceNetwork(string networkName, DateTime timestamp)
    {
        // 1) Prefer the given rangeImporter data
        var hasCacheId = _pricingCache.HasItem(networkName, timestamp);
        if (hasCacheId) return networkName;

        // 2) Didn't find data via given rangeImporter, fallback to and try the others
        foreach (var exchange in _exchanges.Where(x => x.Name != networkName))
        {
            hasCacheId = _pricingCache.HasItem(exchange.Name, timestamp);
            if (hasCacheId) return exchange.Name;
        }

        return string.Empty;
    }
    
    private async Task<ICollection<ExchangeTransaction>> BuildTransactionsFromImporter()
    {
        // This method pulls transactions into a list, and forces iterating over
        // each item. In a slightly more advanced program this code should work
        // with batches / sliding windows and smaller or no lists at all.
        var allTransactions = new List<ExchangeTransaction>();
        foreach (var singleExchange in _exchanges)
        {
            var transactions = await singleExchange.QueryPastTransactions(
                _options.AssetBase!,
                _options.AssetQuote!,
                _options.TimeStart,
                _options.TimeEnd);

            allTransactions.AddRange(transactions);
        }

        return _processor
            .Process(allTransactions)
            .ToList();
    }

    private ICollection<AssetPair> BuildPairCombinations(
        ICollection<ExchangeTransaction> transactions)
    {
        var pairings = new List<string>();
        var allSymbols = transactions
            .Select(tx => tx.Amount.MarketSymbol)
            .Distinct();

        pairings.AddRange(allSymbols);
        if (!string.IsNullOrWhiteSpace(_options.TargetSymbol))
        {
            pairings.Add(_options.TargetSymbol);
        }

        // Generate pairs of 2 combinations (e.g. [BTC, USDT], [BTC, EUR], etc)
        return pairings
            .GetCombinations(2)
            .Select(x => new AssetPair(x.First(), x.Last()))
            .ToList();
    }

    private async Task<ICollection<ExchangePrice>> BuildPricesFromImporter(
        IAssetRangeImporter rangeImporter,
        string baseAsset,
        string quoteAsset)
    {
        // Data was not yet cached. Try to load as fresh data and save it
        // for next time. Make the query broad so data isn't required to
        // reload too often.
        var pastPrices = await rangeImporter
            .QueryPastPrices(
                baseAsset,
                quoteAsset,
                _options.TimeStart,
                _options.TimeEnd);

        pastPrices.ForEach(_pricingCache.Save);
        return pastPrices.ToList();
    }
}