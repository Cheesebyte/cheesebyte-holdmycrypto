using System.Web;

namespace Cheesebyte.HoldMyCrypto.Importers.Utils;

/// <summary>
/// Extension methods for <see cref="HttpClient"/> and related objects.
/// </summary>
public static class HttpMessageExtensions
{
    /// <summary>
    /// Allows chaining <see cref="DelegatingHandler"/> as a pipeline
    /// of handlers. Useful if you're defining <see cref="HttpClient"/>
    /// in a client app instead of server-side as a (micro)service
    /// with the usual setup via a factory and dependency injection
    /// with fluent methods (e.g. `AddHttpMessageHandler`).
    /// <remarks>
    /// <para>
    /// The handlers are handled in reverse - keep this in mind when
    /// defining a pipeline with a set of multiple handlers.
    /// </para>
    /// <para>
    /// Inspired by: https://thomaslevesque.com/2016/12/08/fun-with-the-httpclient-pipeline/
    /// </para>
    /// </remarks>
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="chainedHandler"></param>
    /// <example>
    /// <code>
    /// var pipeline = new HttpClientHandler()
    ///     .DecorateWith(new MyHandler2())
    ///     .DecorateWith(new MyHandler1());
    /// var client = new HttpClient(pipeline);
    /// </code>
    /// </example>
    /// <returns>
    /// A <see cref="DelegatingHandler"/> to be used as a parameter
    /// when creating a new <see cref="HttpClient"/>.
    /// </returns>
    public static DelegatingHandler DecorateWith(
        this HttpClientHandler handler,
        DelegatingHandler chainedHandler)
    {
        chainedHandler.InnerHandler = handler;
        return chainedHandler;
    }

    /// <summary>
    /// See <see cref="DecorateWith(System.Net.Http.HttpClientHandler,System.Net.Http.DelegatingHandler)"/>.
    /// </summary>
    public static DelegatingHandler DecorateWith(
        this DelegatingHandler handler,
        DelegatingHandler chainedHandler)
    {
        chainedHandler.InnerHandler = handler;
        return chainedHandler;
    }

    /// <summary>
    /// Transforms the input query string to a dictionary.
    /// </summary>
    /// <param name="urlQueryString">
    /// The query string in the following format: "option1=value&amp;etc=value".
    /// </param>
    /// <returns>
    /// A new instance of a dictionary with the unpacked key-value pairs
    /// from <see cref="urlQueryString"/>.
    /// </returns>
    public static IDictionary<string, string?> QueryToDictionary(this string urlQueryString)
    {
        var nameValueCollection = HttpUtility.ParseQueryString(urlQueryString);
        var nameValueDictionary = nameValueCollection
            .AllKeys
            .ToDictionary(
                k => k!.ToString(),
                k => nameValueCollection[k]);

        return nameValueDictionary;
    }
}
