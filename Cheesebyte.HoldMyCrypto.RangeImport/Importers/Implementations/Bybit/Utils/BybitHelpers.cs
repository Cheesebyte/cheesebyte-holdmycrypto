using System.Collections.Specialized;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using MoreLinq.Extensions;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Utils;

/// <summary>
/// Helpers to prepare and sign data for the Bybit API, as well as
/// functionality to handle special cases from older API versions.
/// </summary>
internal static class BybitHelpers
{
    /// <summary>
    /// https://github.com/bybit-rangeImporter/bybit-official-api-docs/blob/master/en/rest_api_sign.md#how-to-sign
    /// </summary>
    /// <param name="secret"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string CreateSignature(string secret, string message)
    {
        var signatureBytes = ConvertMessageToHash(
            Encoding.UTF8.GetBytes(secret),
            Encoding.UTF8.GetBytes(message));

        return ByteArrayToString(signatureBytes);
    }
    
    private static byte[] ConvertMessageToHash(byte[] keyByte, byte[] messageBytes)
    {
        using var hash = new HMACSHA256(keyByte);
        return hash.ComputeHash(messageBytes);
    }

    private static string ByteArrayToString(byte[] byteArray)
    {
        var hex = new StringBuilder(byteArray.Length * 2);
        byteArray.ForEach(b => hex.AppendFormat("{0:x2}", b));
        return hex.ToString();
    }

    public static string BuildQueryString(IDictionary<string, string?> queryDict)
    {
        return BuildQueryString(queryDict.ToList());
    }

    public static string BuildQueryString(NameValueCollection nvc)
    {
        var items = nvc.AllKeys.SelectMany(
            nvc.GetValues!,
            (k, v) => new KeyValuePair<string, string?>(k ?? string.Empty, v));

        return BuildQueryString(items.ToList());
    }

    public static string BuildQueryString(ICollection<KeyValuePair<string, string?>> queryPairs)
    {
        var pairs = queryPairs
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key))
            .Select(kvp => $"{kvp.Key}={kvp.Value}");

        return string.Join("&", pairs);
    }

    public static string FormatOldApiDate(DateTime dateToFormat)
    {
        var dateApiFormat = "yyyy-MM-dd";
        var dateFormatted = dateToFormat.ToString(dateApiFormat);

        return dateFormatted;
    }

    public static ICollection<KeyValuePair<string, string?>> SignRequest(
        string apiKey,
        string apiSecret,
        IDictionary<string, string?> options)
    {
        var milliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var timestamp = milliseconds.ToString(CultureInfo.InvariantCulture);

        var sortedOptions = new SortedDictionary<string, string?>(options);
        sortedOptions.Add("api_key", apiKey);
        sortedOptions.Add("timestamp", timestamp);
        sortedOptions.Add("recv_window", "5000");

        var formattedParameters = BuildQueryString(sortedOptions);
        var sign = CreateSignature(apiSecret, formattedParameters);

        // Bybit wants these query params to be sorted alphabetically with
        // the exception of the sign parameter. This signature should be
        // added last.
        //
        // That's what this new list is for; to allow adding the sign as
        // the last query parameter without having SortedDictionary
        // accidentally interfere with param order. Lists are guaranteed
        // to keep the same order as defined and thus are a better match
        // for this work without the need to use odd workarounds.
        var queryParams = sortedOptions.ToList();
        queryParams.Add(new KeyValuePair<string, string?>("sign", sign));

        return queryParams;
    }
}