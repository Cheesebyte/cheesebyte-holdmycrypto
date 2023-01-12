using Cheesebyte.HoldMyCrypto.Console.Commands.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cheesebyte.HoldMyCrypto.Console.Commands;

/// <summary>
/// Simple action to wait for the user to press a key to continue.
/// </summary>
public class WaitForInputCommand : ICommand
{
    private readonly ILogger _logger;

    public WaitForInputCommand(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DisplayTransactionCommand>();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Task Run()
    {
        _logger.LogInformation("Waiting. Press <Enter> key when you're done...");

        System.Console.ReadLine();
        return Task.CompletedTask;
    }
}