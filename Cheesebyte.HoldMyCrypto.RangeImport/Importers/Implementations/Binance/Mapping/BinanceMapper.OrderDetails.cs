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
    /// <param name="orders">Binance order info.</param>
    /// <param name="trades">Binance trade info.</param>
    /// <returns>A list of transformed transactions.</returns>
    public IEnumerable<ExchangeTransaction> TxFromOrderDetails(
        ICollection<BinanceOrder> orders,
        ICollection<BinanceTrade> trades)
    {
        if (!orders.Any())
        {
            return Enumerable.Empty<ExchangeTransaction>();
        }

        // Create transactions for buy/sell as double entries
        // on both sides (base and quote = from and to).
        var orderTransactions = orders
            .Where(x => x.Status is OrderStatus.Filled or OrderStatus.PartiallyFilled)
            .SelectMany(order => BuildTransactions(order, trades));

        return orderTransactions;
    }

    private IEnumerable<ExchangeTransaction> BuildTransactions(
        BinanceOrder order,
        IEnumerable<BinanceTrade> trades)
    {
        {
            var bq = AssetUtils.GetBaseQuoteFromSymbol(order.Symbol);
            var qFrom = order.Side == OrderSide.Buy ? bq.Quote : bq.Base;
            var qTo = order.Side == OrderSide.Buy ? bq.Base : bq.Quote;
            var fee = GetFeeForOrders(qTo, new[] { order }, trades);

            // QuantityFilled = executed quantity
            // QuoteQuantityFilled = cumulative quote quantity
            var assetAccountNameTo = AssetUtils.ExtractAccountName(qTo);
            var txToQuantity = order.Side == OrderSide.Buy ? order.QuantityFilled : order.QuoteQuantityFilled;
            var txTo = new ExchangeTransaction(
                order.CreateTime,
                RangeImporter.Name,
                order.Id.ToString(),
                new ExchangeAmount(order.CreateTime, RangeImporter.Name, qTo, txToQuantity),
                new ExchangeNetwork(assetAccountNameTo, $"{order.Id}"),
                ExchangeCategory.Asset,
                ExchangeTransactionType.Credit);

            var txToFee = new ExchangeTransaction(
                order.CreateTime,
                RangeImporter.Name,
                order.Id.ToString(),
                new ExchangeAmount(order.CreateTime, RangeImporter.Name, qTo, fee),
                new ExchangeNetwork(AssetUtils.ExtractAccountName(qTo), $"{order.Id}"),
                ExchangeCategory.Fee,
                ExchangeTransactionType.Debit);

            // Create the double entry for each transaction created for an
            // order, because all transactions happened on the same rangeImporter.
            // For withdrawal and deposit, double entries are created by
            // other implementations of IExchangeRepository.
            var txFromQuantity = order.Side == OrderSide.Sell ?
                order.QuantityFilled :
                order.QuoteQuantityFilled;

            var txFrom = new ExchangeTransaction(
                order.CreateTime,
                RangeImporter.Name,
                order.Id.ToString(),
                new ExchangeAmount(order.CreateTime, RangeImporter.Name, qFrom, txFromQuantity),
                new ExchangeNetwork(AssetUtils.ExtractAccountName(qFrom), $"{order.Id}"),
                ExchangeCategory.Expense,
                ExchangeTransactionType.Debit);

            return new[] { txTo, txFrom, txToFee };
        }
    }
}