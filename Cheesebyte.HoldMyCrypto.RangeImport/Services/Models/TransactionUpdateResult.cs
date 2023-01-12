using Cheesebyte.HoldMyCrypto.Services.Interfaces;

namespace Cheesebyte.HoldMyCrypto.Services.Models;

/// <summary>
/// Result saving model for methods in <see cref="ILedgerService"/>.
/// </summary>
public class TransactionUpdateResult
{
    public int RetrievedCount { get; set; }
}