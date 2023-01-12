namespace Cheesebyte.HoldMyCrypto.Caching.Interfaces;

/// <summary>
/// Allows to different ways to implement a quick internal cache.
/// </summary>
/// <typeparam name="TItem">Item type to support caching for.</typeparam>
public interface IItemCache<TItem>
{
    /// <summary>
    /// <para>
    /// This method is not awaitable by design. When its functionality is used
    /// the caller most often doesn't want it to be awaitable because answers
    /// need to be delivered asap.
    /// </para>
    /// <para>
    /// Making this async would defeat the purpose of being able to check
    /// the cache to know whether the caller should continue performing more
    /// expensive operations.
    /// </para>
    /// </summary>
    /// <param name="networkName">
    /// Network origin name (e.g. exchange name).
    /// </param>
    /// <param name="timestamp">Exact position of the data point in time.</param>
    /// <returns>True if successful.</returns>
    bool HasItem(string networkName, DateTime timestamp);

    /// <summary>
    /// Saves the properties of <paramref name="item"/>.
    /// </summary>
    /// <param name="item">Item to save.</param>
    void Save(TItem item);

    /// <summary>
    /// Loads the properties of the asset as indicated by timestamp and network name.
    /// </summary>
    /// <param name="networkName">
    /// Network origin name (e.g. exchange name).
    /// </param>
    /// <param name="timestamp">Exact position of the data point in time.</param>
    /// <returns>
    /// An <see cref="TItem"/> object with the requested data.
    /// </returns>
    TItem? Load(string networkName, DateTime timestamp);

    /// <summary>
    /// Same as <see cref="Load"/> but loads a full range of items instead.
    /// </summary>
    /// <param name="networkName">
    /// Network origin name (e.g. exchange name).
    /// </param>
    /// <returns>
    /// An array of <see cref="TItem"/> objects with the requested data.
    /// </returns>
    IEnumerable<TItem> LoadRange(string networkName);
}