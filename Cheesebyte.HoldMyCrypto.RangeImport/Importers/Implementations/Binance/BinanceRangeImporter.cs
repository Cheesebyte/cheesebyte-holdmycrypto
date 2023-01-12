using Ardalis.GuardClauses;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot;
using Cheesebyte.HoldMyCrypto.Importers.Implementations.Binance.Mapping;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Vaults.Interfaces;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Binance;

/// <summary>
/// Produces internal and normalised models for data from <a href="https://binance.com">Binance</a>.
/// <list type="bullet">
/// <item>https://www.binance.com/en/my/settings/api-management</item>
/// </list>
/// </summary>
public class BinanceRangeImporter : IAssetRangeImporter
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name => "Binance";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool HasFullHistory => false;

    private readonly BinanceClient _client;
    private readonly BinanceMapper _mapper;

    public BinanceRangeImporter(ISecretVault secretVault)
    {
        var apiKey = secretVault.GetSecureKey<string>($"{Name}/Key") ?? string.Empty;
        var apiSecret = secretVault.GetSecureKey<string>($"{Name}/Secret") ?? string.Empty;

        Guard.Against.NullOrEmpty(apiKey, "Missing Binance API key");
        Guard.Against.NullOrEmpty(apiSecret, "Missing Binance API secret");
        
        // Rate limiting is handled internally via SpotApiOptions (see Binance.Net code)
        BinanceClient.SetDefaultOptions(new BinanceClientOptions
        {
            ApiCredentials = new ApiCredentials(apiKey, apiSecret),
            LogLevel = LogLevel.Information,
        });

        _client = new BinanceClient();
        _mapper = new BinanceMapper(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask<IEnumerable<ExchangeTransaction>> QueryPastTransactions(
        string baseAsset,
        string quoteAsset,
        DateTime? dateStart,
        DateTime? dateEnd = null)
    {
        var symbol = _mapper.GenerateMarketSymbol(baseAsset, quoteAsset);

        // Make sure not to use null for the end date. It results in zero
        // entries for the LINQ statements below.
        dateEnd ??= DateTime.UtcNow;

        // For deposits and withdrawals, limiting date start and end is
        // done via LINQ because the Binance REST API eeks out whenever
        // 'dateStart' is passed as an argument. Not sure why. The message
        // returned is: "3: Server error: StartTime Or EndTime Error".
        var clientDeposits = await _client.SpotApi.Account.GetDepositHistoryAsync(asset: null, status: DepositStatus.Success);
        var clientWithdrawals = await _client.SpotApi.Account.GetWithdrawalHistoryAsync(asset: null, status: WithdrawalStatus.Completed);

        var deposits = clientDeposits
            .Data
            .Where(x => x.InsertTime >= dateStart && x.InsertTime < dateEnd)
            .ToList();

        var withdrawals = clientWithdrawals
            .Data
            .Where(x => x.ApplyTime >= dateStart && x.ApplyTime < dateEnd)
            .ToList();

        var orders = await AcquireBinanceOrders(symbol, dateStart, dateEnd);
        var trades = await AcquireBinanceTrades(symbol, dateStart, dateEnd);

        var orderTransactions = _mapper.TxFromOrderDetails(orders, trades);
        var depositTransactions = _mapper.TxFromDeposits(deposits);
        var withdrawalTransactions = _mapper.TxFromWithdrawals(withdrawals);

        return orderTransactions
            .Concat(depositTransactions)
            .Concat(withdrawalTransactions)
            .OrderBy(tx => tx.Timestamp);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask<IEnumerable<ExchangePrice>> QueryPastPrices(
        string baseAsset,
        string quoteAsset,
        DateTime? dateStart,
        DateTime? dateEnd = null)
    {
        var symbol = _mapper.GenerateMarketSymbol(baseAsset, quoteAsset);
        var trades = await AcquireBinanceTrades(symbol, dateStart, dateEnd);

        // Getting this info from anywhere else would be expensive, so we're
        // reusing the trades here. This should give a close approximation of
        // what the actual price would have been at the given time when it's
        // only used to get prices at known transaction times.
        var pastPrices = trades
            .Where(trade => trade.Symbol == symbol)
            .Where(trade => trade.Timestamp >= dateStart && trade.Timestamp < dateEnd)
            .Select(trade => new ExchangePrice(
                trade.Timestamp,
                Name,
                baseAsset,
                new ExchangeAmount(
                    trade.Timestamp,
                    Name,
                    symbol,
                    trade.Price)));

        return pastPrices;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsSymbolPairSupported(string baseAsset, string quoteAsset) =>
        _mapper.IsSymbolPairSupported(baseAsset, quoteAsset);

    private async ValueTask<ICollection<BinanceOrder>> AcquireBinanceOrders(
        string symbol,
        DateTime? dateStart,
        DateTime? dateEnd)
    {
        // Passing both the start and end time to the Binance REST API
        // was tested, but does not work for retrieving trades and orders.
        // Documentation says: "If both startTime and endTime are sent, time
        // between startTime and endTime must be less than 1 hour."
        var clientOrders = await _client
            .SpotApi
            .Trading
            .GetOrdersAsync(symbol: symbol, startTime: dateStart);

        var orders = clientOrders
            .Data
            .Where(x => x.CreateTime < dateEnd)
            .ToList();

        return orders;
    }

    private async ValueTask<ICollection<BinanceTrade>> AcquireBinanceTrades(
        string symbol,
        DateTime? dateStart,
        DateTime? dateEnd)
    {
        var clientTrades = await _client
            .SpotApi
            .Trading
            .GetUserTradesAsync(symbol: symbol, startTime: dateStart);

        var trades = clientTrades
            .Data
            .Where(x => x.Timestamp < dateEnd)
            .ToList();

        return trades;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Dispose()
    {
        _client.Dispose();
    }
}