using System.Text.Json;
using Cheesebyte.HoldMyCrypto.Importers.Interfaces;

namespace Cheesebyte.HoldMyCrypto.Caching.Utils;

/// <summary>
/// Helper utilities to handle JSON and <see cref="JsonCache{TItem}"/>
/// specific functionality to keep other objects SRP.
/// </summary>
internal static class JsonUtils
{
    internal static void SaveJson<TItem>(TItem item, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(item, options);
        File.WriteAllText(filePath, json);
    }

    internal static TItem? LoadJson<TItem>(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return !string.IsNullOrWhiteSpace(json) ?
            JsonSerializer.Deserialize<TItem>(json) :
            default;
    }
    
    internal static void PreCreatePathsOnDisk(
        string basePath,
        string itemDirectory,
        IEnumerable<IAssetRangeImporter> exchanges)
    {
        Directory.CreateDirectory(basePath);
        foreach (var singleExchange in exchanges)
        {
            var cacheFilePath = Path.Combine(basePath, itemDirectory, singleExchange.Name);

            if (string.IsNullOrWhiteSpace(cacheFilePath)) return;
            Directory.CreateDirectory(cacheFilePath);
        }
    }
}