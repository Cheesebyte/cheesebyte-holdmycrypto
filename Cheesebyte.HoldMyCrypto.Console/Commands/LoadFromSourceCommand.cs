using Cheesebyte.HoldMyCrypto.Caching.Interfaces;
using Cheesebyte.HoldMyCrypto.Console.Commands.Interfaces;
using Cheesebyte.HoldMyCrypto.Importers.Exceptions;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cheesebyte.HoldMyCrypto.Console.Commands;

/// <summary>
/// Requests data from each supported <see cref="IAssetRangeImporter"/>
/// and saves it via the internal <see cref="IItemCache{T}"/>.
/// </summary>
public class LoadFromSourceCommand : ICommand
{
    private readonly ILedgerService _ledgerService;
    private readonly ILogger _logger;

    public LoadFromSourceCommand(
        ILedgerService ledgerService,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DisplayTransactionCommand>();
        _ledgerService = ledgerService;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task Run()
    {
        try
        {
            await UpdateFromSource();
        }
        catch (ExchangeException ex)
        {
            // Differentiate between "just zero results" and "something is wrong"
            // by catching exceptions from the ledger service or a range importer.
            _logger.LogError(ex.Message, ex);
        }

        System.Console.ReadLine();
    }

    private async Task UpdateFromSource()
    {
        _logger.LogInformation("Preparing import...");

        var resultUsdt = await _ledgerService.UpdateAllFromSource();
        _logger.LogInformation($"{resultUsdt.RetrievedCount} transactions imported");
    }
}