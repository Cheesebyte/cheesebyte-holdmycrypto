namespace Cheesebyte.HoldMyCrypto.Models;

/// <summary>
/// Anything that can be considered a repository or source of value, such
/// as banks, crypto or rangeImporter. By considering these to be the same
/// basic concept, they can be treated as a uniform data source.
/// </summary>
/// <param name="Name">
/// Bank name, crypto or blockchain name, importer name, etc.
/// </param>
/// <param name="Address">
/// Account number, crypto address, etc.
/// </param>
public record ExchangeNetwork(string Name, string Address);