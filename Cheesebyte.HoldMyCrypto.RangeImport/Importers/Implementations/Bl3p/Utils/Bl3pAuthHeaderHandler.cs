using System.Globalization;
using Ardalis.GuardClauses;
using Cheesebyte.HoldMyCrypto.Importers.Utils;
using Cheesebyte.HoldMyCrypto.Vaults.Interfaces;
using Polly;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Utils;

/// <summary>
/// Adds authentication functionality before sending a network request.
/// </summary>
public class Bl3pAuthHeaderHandler : DelegatingHandler
{
    private readonly ISecretVault _secretVault;

    public Bl3pAuthHeaderHandler(ISecretVault secretVault)
    {
        Guard.Against.Null(secretVault, "Vault was not injected");

        _secretVault = secretVault;
        InnerHandler = new HttpClientHandler();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestUri = request.RequestUri;
        var publicKey = _secretVault.GetSecureKey<string>("Bl3p/Key") ?? string.Empty;
        var secretKey = _secretVault.GetSecureKey<string>("Bl3p/Secret") ?? string.Empty;

        Guard.Against.Null(requestUri, "Missing request URI");
        Guard.Against.NullOrEmpty(publicKey, "Missing BL3P key");
        Guard.Against.NullOrEmpty(secretKey, "Missing BL3P secret");
        
        // Add rate limiting via Polly, see: https://github.com/reactiveui/refit/issues/801
        var pollyContext = new Context();
        request.SetPolicyExecutionContext(pollyContext);

        // BL3P expects a nonce with a precision of 16 digits, therefore
        // milliseconds wouldn't be enough. Add microseconds into the mix.
        var time = DateTimeOffset.UtcNow;
        var nonce = time.ToUnixTimeSeconds() * 1000 * 1000 + time.Microsecond;
        var inputQueryArgs = requestUri.Query.QueryToDictionary();
        inputQueryArgs.Add("nonce", nonce.ToString(CultureInfo.InvariantCulture));

        // Skipping the first 2 to remove ('/1') for the version is potentially dangerous - fix
        var path = string.Join(string.Empty, requestUri.Segments.Skip(2));
        var sign = SignatureHelpers.SignWithPrivateKey(secretKey, path, inputQueryArgs);

        request.Headers.Add("Rest-Key", publicKey);
        request.Headers.Add("Rest-Sign", sign);
        request.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; BL3P API client; 0.1)");

        // Clear the query arguments as passed into Refit client and post
        // the data instead. Feels odd, because it is, but Refit appears
        // to support [Body(BodySerializationMethod.UrlEncoded)] for only
        // one (dictionary) parameter instead of multiple params with
        // varying types.
        request.Content = new FormUrlEncodedContent(inputQueryArgs);
        var updatedUri = requestUri
            .AbsoluteUri
            .Replace(requestUri.Query, string.Empty);

        request.RequestUri = new Uri(updatedUri);
        return await base
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}