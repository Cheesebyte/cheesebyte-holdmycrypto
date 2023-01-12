using Ardalis.GuardClauses;
using Cheesebyte.HoldMyCrypto.Importers.Utils;
using Cheesebyte.HoldMyCrypto.Vaults.Interfaces;
using Polly;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bybit.Utils;

/// <summary>
/// Adds authentication functionality before sending a network request.
/// </summary>
public class BybitAuthHeaderHandler : DelegatingHandler
{
    private readonly ISecretVault _secretVault;

    public BybitAuthHeaderHandler(ISecretVault secretVault)
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
        var publicKey = _secretVault.GetSecureKey<string>("Bybit/Key") ?? string.Empty;
        var secretKey = _secretVault.GetSecureKey<string>("Bybit/Secret") ?? string.Empty;

        Guard.Against.Null(requestUri, "Missing request URI");
        Guard.Against.NullOrEmpty(publicKey, "Missing Bybit key");
        Guard.Against.NullOrEmpty(secretKey, "Missing Bybit secret");

        // Add rate limiting via Polly, see: https://github.com/reactiveui/refit/issues/801
        var pollyContext = new Context();
        request.SetPolicyExecutionContext(pollyContext);

        // Code below is for GET methods only. Add a JSON content-body for
        // POST requests with the same query params.
        var inputQueryArgs = requestUri.Query.QueryToDictionary();
        var authorisedQueryArgs = BybitHelpers.SignRequest(publicKey, secretKey, inputQueryArgs);
        var updatedUri = requestUri
            .AbsoluteUri
            .Replace(requestUri.Query, $"?{BybitHelpers.BuildQueryString(authorisedQueryArgs)}");

        // Bybit wants headers for more asset account data from their newer APIs
        // See: https://bybit-rangeImporter.github.io/docs/account_asset/v3/#t-constructingtherequest
        // request.Headers.Add("X-BAPI-SIGN-TYPE", "2");
        // request.Headers.Add("X-BAPI-API-KEY", settings.Key);
        // request.Headers.Add("X-BAPI-SIGN", authorisedQueryParams.SingleOrDefault(x => x.Key == "sign").Value);
        // request.Headers.Add("X-BAPI-TIMESTAMP", authorisedQueryParams.SingleOrDefault(x => x.Key == "timestamp").Value);
        // request.Headers.Add("X-BAPI-RECV-WINDOW", "5000");

        request.RequestUri = new Uri(updatedUri);
        return await base
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}