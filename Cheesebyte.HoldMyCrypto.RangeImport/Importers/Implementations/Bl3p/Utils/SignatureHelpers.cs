using System.Security.Cryptography;
using System.Text;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Utils;

/// <summary>
/// Helper for signing BL3P API requests.
/// </summary>
public static class SignatureHelpers
{
    /// <summary>
    /// Signs the endpoint action with input query parameters to make them
    /// usable for sending the data safely towards the BL3P API. It confirms
    /// to BL3P that the current API user is legit.
    /// </summary>
    /// <param name="secretKey">The secret key as it was provided by BL3P.</param>
    /// <param name="action">Endpoint action to call with these query params.</param>
    /// <param name="queryParams">Query parameters to send along with the call.</param>
    /// <returns>A signature that can be used to pass as a header to a BL3P API call.</returns>
    public static string SignWithPrivateKey(
        string secretKey,
        string action,
        IDictionary<string, string?> queryParams)
    {
        // Why not HttpUtility.UrlEncode? Can't remember, it's code from the past.
        var postData = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        var body = $"{action}{char.MinValue}{postData}";
        var bodyBytes = Encoding.UTF8.GetBytes(body);

        var secretKeyBytes = Convert.FromBase64String(secretKey);
        using var hash = new HMACSHA512(secretKeyBytes);
        var signatureBytes = hash.ComputeHash(bodyBytes);

        return Convert.ToBase64String(signatureBytes);
    }
}