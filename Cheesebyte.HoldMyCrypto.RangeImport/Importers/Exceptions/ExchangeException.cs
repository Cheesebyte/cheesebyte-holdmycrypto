using Cheesebyte.HoldMyCrypto.Importers.Interfaces;

namespace Cheesebyte.HoldMyCrypto.Importers.Exceptions;

/// <summary>
/// Exception for <see cref="IAssetRangeImporter"/>.
/// </summary>
public class ExchangeException : Exception
{
    public ExchangeException() : base($"An {nameof(IAssetRangeImporter)}-based implementation has thrown an exception.") { }
    public ExchangeException(string message) : base(message) { }
    public ExchangeException(string message, Exception exception) : base (message, exception) { }
}