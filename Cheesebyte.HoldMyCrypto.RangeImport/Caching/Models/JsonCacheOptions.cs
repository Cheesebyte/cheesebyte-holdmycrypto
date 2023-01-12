namespace Cheesebyte.HoldMyCrypto.Caching.Models;

/// <summary>
/// Options object for <see cref="JsonCache{T}"/>.
/// </summary>
public class JsonCacheOptions
{
    /// <summary>
    /// Path to use as the root for saving cached data to.
    /// </summary>
    public string? BaseCachePath { get; init; }
}