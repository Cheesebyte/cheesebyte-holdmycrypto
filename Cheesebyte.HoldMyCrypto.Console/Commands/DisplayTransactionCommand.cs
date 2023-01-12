using Ardalis.GuardClauses;
using Cheesebyte.HoldMyCrypto.Console.Commands.Interfaces;
using Cheesebyte.HoldMyCrypto.Console.Extensions;
using Cheesebyte.HoldMyCrypto.Enums;
using Cheesebyte.HoldMyCrypto.Importers.Exceptions;
using Cheesebyte.HoldMyCrypto.Models;
using Cheesebyte.HoldMyCrypto.Services.Interfaces;
using Cheesebyte.HoldMyCrypto.Services.Models;
using Cheesebyte.HoldMyCrypto.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cheesebyte.HoldMyCrypto.Console.Commands;

/// <summary>
/// Prints a list of transactions with full details, as requested via
/// the <see cref="TransactionOptions"/> object.
/// </summary>
public class DisplayTransactionCommand : ICommand
{
    private DateTime? _transactionDateStart;
    private DateTime? _transactionDateEnd;

    private readonly string _convertToBaseSymbol;
    private readonly string[] _tableTransactionHeaders;

    private readonly ILedgerService _transactionService;
    private readonly ILogger _logger;

    private const int CurrencyPadding = 10;

    public DisplayTransactionCommand(
        ILedgerService transactionService,
        ILoggerFactory loggerFactory,
        IOptions<TransactionOptions> options)
    {
        Guard.Against.Null(options.Value.TargetSymbol, "Target symbol is missing");

        _convertToBaseSymbol = options.Value.TargetSymbol;
        _transactionDateStart = options.Value.TimeStart;
        _transactionDateEnd = options.Value.TimeEnd;

        _logger = loggerFactory.CreateLogger<DisplayTransactionCommand>();
        _transactionService = transactionService;
        _tableTransactionHeaders = new[]
        {
            "Category",
            "Quantity",
            "Asset",
            $"In {_convertToBaseSymbol}",
            "Date",
            "Side",
            "Source",
            "Tx ID",
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task Run()
    {
        try
        {
            DisplayTransactionsFromCache();
        }
        catch (ExchangeException ex)
        {
            // Differentiate between "just zero results" and "something is wrong"
            // by catching exceptions from the transaction service or any repository.
            _logger.LogError(ex.Message, ex);
        }

        return Task.CompletedTask;
    }

    private void DisplayTransactionsFromCache()
    {
        var transactions = _transactionService.QueryTransactions();
        transactions
            .OrderBy(tx => tx.Timestamp)
            .GroupBy(tx => tx.Timestamp.Date)
            .Select(tx => tx.ToList())
            .ToList()
            .ForEach(DisplayTransactionsForDay);
    }

    private (
        ExchangeAmount totalFee,
        ExchangeAmount totalAssets,
        ExchangeAmount totalExpenses)
        CalculateAmounts(IEnumerable<ExchangeTransaction> transactionsInDay)
    {
        var converter = _transactionService.ConversionProfile;
        var currentDate = transactionsInDay.First().Timestamp.Date;

        var totalFee = TransactionUtils.CalculateTotalAmount(transactionsInDay, converter, currentDate, _convertToBaseSymbol, ExchangeCategory.Fee);
        var totalAssets = TransactionUtils.CalculateTotalAmount(transactionsInDay, converter, currentDate, _convertToBaseSymbol, ExchangeCategory.Asset);
        var totalExpenses = TransactionUtils.CalculateTotalAmount(transactionsInDay, converter, currentDate, _convertToBaseSymbol, ExchangeCategory.Expense);

        return (totalFee, totalAssets, totalExpenses);
    }

    private void DisplayFormattedAmounts(
        ExchangeAmount totalFee,
        ExchangeAmount totalAssets,
        ExchangeAmount totalExpenses)
    {
        // This will currently not work for transactions from Bybit or Bl3p
        // because its imported transactions are neither complete nor balanced.
        var totalAssetsRemainder = totalAssets - totalExpenses;
        var none = "None".PadLeft(CurrencyPadding);

        System.Console.WriteLine($"   Fees:\t{totalFee.ToString().PadLeft(CurrencyPadding)}");
        System.Console.WriteLine($"   Added:\t{(totalAssetsRemainder.Quantity < 0m ? none : totalAssetsRemainder.ToString().PadLeft(CurrencyPadding))}");
        System.Console.WriteLine($"   Expenses:\t{(totalAssetsRemainder.Quantity < 0m ? totalAssetsRemainder.ToString().PadLeft(CurrencyPadding) : none)}");
    }

    private void DisplayTransactionsForDay(IEnumerable<ExchangeTransaction> transactionsInDay)
    {
        // Calculate all fees for this day, which includes realised P&L (e.g. profit from
        // keeping an open position). Let the converter decide what asset to use for
        // conversion output.
        var currentDate = transactionsInDay.First().Timestamp.Date;
        var amounts = CalculateAmounts(transactionsInDay);

        ConsoleExtensions.WriteDivider();
        System.Console.WriteLine($" * Totals on {currentDate.ToShortDateString()}:");
        ConsoleExtensions.WriteDivider();
        DisplayFormattedAmounts(amounts.totalFee, amounts.totalAssets, amounts.totalExpenses);
        System.Console.WriteLine();

        // Output a formatted table with the transactions for this day
        DisplayFormattedTransactions(transactionsInDay);
    }

    private void DisplayFormattedTransactions(IEnumerable<ExchangeTransaction> transactionsInDay)
    {
        var converter = _transactionService.ConversionProfile;
        var body = transactionsInDay
            .OrderBy(tx => tx.Category)
            .ToStringTable(_tableTransactionHeaders,
                a => a.Category,
                a => a.Amount.ToString(),
                a => a.Amount.MarketSymbol,
                a => converter.Convert(a.Amount, a.Timestamp, _convertToBaseSymbol).ToString().PadLeft(CurrencyPadding),
                a => a.Timestamp.ToShortTimeString(),
                a => a.Type,
                a => a.SourceName,
                a => a.TransactionId.Truncate(10)!);

        System.Console.WriteLine(body);
        System.Console.WriteLine();
    }
}