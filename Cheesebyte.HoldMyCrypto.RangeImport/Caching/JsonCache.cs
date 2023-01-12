using Cheesebyte.HoldMyCrypto.Caching.Interfaces;
using Cheesebyte.HoldMyCrypto.Caching.Models;
using Cheesebyte.HoldMyCrypto.Caching.Utils;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;
using Cheesebyte.HoldMyCrypto.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cheesebyte.HoldMyCrypto.Caching;

/// <summary>
/// Poor man's <see cref="IItemCache{TItem}"/>. The intention here is
/// to have *something* for the time being, but not to keep this for
/// production quality results. Use Redis or choose depending on
/// how the program's structure works out with microservices and
/// putting the data into a system like Apache Kafka's streams.
/// <inheritdoc/>
/// </summary>
public class JsonCache<TItem> : IItemCache<TItem>
    where TItem : BaseExchangeTimeData
{
    private readonly Dictionary<string, TItem> _cache;
    private readonly string _basePath;
    private readonly ILogger _logger;

    private static string ItemName => typeof(TItem).Name;
    private const string JsonFileExtension = "txt";

    public JsonCache(
        ILoggerFactory loggerFactory,
        IEnumerable<IAssetRangeImporter> importers,
        IOptions<JsonCacheOptions> options,
        IValidator<JsonCacheOptions> validator)
    {
        validator.ValidateAndThrow(options.Value);

        // Null-forgiving because compiler cannot see that path was validated
        _basePath = options.Value.BaseCachePath!;
        _cache = new Dictionary<string, TItem>();
        _logger = loggerFactory.CreateLogger<JsonCache<TItem>>();

        // Ensure that all paths to save data to exist
        JsonUtils.PreCreatePathsOnDisk(_basePath, ItemName, importers);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool HasItem(string networkName, DateTime timestamp)
    {
        if (string.IsNullOrWhiteSpace(networkName))
        {
            return false;
        }

        var cacheFilePath = Path.Combine(_basePath, ItemName, networkName);
        return File.Exists(cacheFilePath);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TItem? Load(string networkName, DateTime timestamp)
    {
        var cacheFilePath = Path.Combine(_basePath, ItemName, networkName);
        return LoadWithPath(cacheFilePath);
    }

    private TItem? LoadWithPath(string cacheFilePath)
    {
        if (_cache.TryGetValue(cacheFilePath, out var cachedItem))
        {
            return cachedItem;
        }

        var item = JsonUtils.LoadJson<TItem>(cacheFilePath);
        if (item == null) return null;

        _cache[cacheFilePath] = item;
        return item;

    }
    
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<TItem> LoadRange(string networkName)
    {
        var cacheFilePath = Path.Combine(_basePath, ItemName, networkName);
        if (!Directory.Exists(cacheFilePath))
        {
            return Enumerable.Empty<TItem>();
        }

        var pattern = $"*.{JsonFileExtension}";
        return Directory
            .EnumerateFiles(cacheFilePath, pattern, SearchOption.AllDirectories)
            .Select(LoadWithPath)
            .Where(x => x != null)
            .Cast<TItem>();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Save(TItem item)
    {
        // NOTE: Usage of NewGuid is bad - should be replaced with persistent hash (but
        // GetHashCode cannot be used because it changes due to safety reasons) to
        // prevent adding new entries on each save.
        var cachePath = Path.Combine(_basePath, ItemName, item.SourceName);
        var cacheId = $"{item.Timestamp.Ticks.ToString()}-{Guid.NewGuid().ToString()}";
        var cacheFilePath = Path.Combine(cachePath, $"{cacheId}.{JsonFileExtension}");

        JsonUtils.SaveJson(item, cacheFilePath);
        _cache[cacheFilePath] = item;
    }
}