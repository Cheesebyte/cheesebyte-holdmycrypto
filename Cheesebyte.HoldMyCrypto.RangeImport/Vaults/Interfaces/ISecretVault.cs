namespace Cheesebyte.HoldMyCrypto.Vaults.Interfaces;

/// <summary>
/// <para>
/// Work in progress for access to private keys. Will be replaced later by
/// a better approach to access these keys. It's important to keep keys
/// abstracted from the start so that it won't interfere with keeping
/// the code maintainable later on and ensures keeping away unintended
/// usage of things like the service locator anti-pattern, or worse;
/// exposing keys as part of the code that uses it.
/// </para>
/// <para>
/// Injecting option pattern models into implementations of
/// <see cref="DelegatingHandler"/> would be problematic due to its independence
/// of code in the presentation or service layer. In addition, key data should
/// not be static to allow easy (and maintainable, code-wise) refresh of keys.
/// </para>
/// </summary>
public interface ISecretVault
{
    /// <summary>
    /// Returns the latest value from a vault for <paramref name="keyName"/>.
    /// </summary>
    /// <param name="keyName">
    /// Key to use for searching data. Use directory-style separator for sections
    /// (e.g. "Cheesebyte/SecretKey").
    /// </param>
    /// <typeparam name="T">
    /// The type to convert the value from the retrieved data to.
    /// </typeparam>
    /// <returns></returns>
    T? GetSecureKey<T>(string keyName);
}