namespace Cheesebyte.HoldMyCrypto.Models;

/// <summary>
/// Common properties for time-related model data.
/// </summary>
/// <param name="Timestamp">
/// Time of the data object. Supports a date and time if it
/// was supported by the source input.
/// </param>
/// <param name="SourceName">
/// Describes where this transaction came from. Null or
/// empty in case of no available importer or other
/// external data source.
/// </param>
public record BaseExchangeTimeData(
    DateTime Timestamp,
    string SourceName
);