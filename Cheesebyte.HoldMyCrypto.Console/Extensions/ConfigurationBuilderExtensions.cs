using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Cheesebyte.HoldMyCrypto.Console.Extensions;

/// <summary>
/// Extension methods for <see cref="IConfigurationBuilder"/>.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// <para>
    /// Converts all properties of an object to
    /// an array of key-value pairs.
    /// </para>
    /// <para>
    /// https://stackoverflow.com/a/49384575
    /// </para>
    /// </summary>
    /// <param name="settings">
    /// The input object to use for reading properties.
    /// </param>
    /// <param name="settingsRoot">
    /// A root name to apply to each key name (e.g. '{settingsRoot}:{keyName}').
    /// </param>
    /// <returns>
    /// An array of <see cref="KeyValuePair{TKey,TValue}"/> objects
    /// containing the property names and values.
    /// </returns>
    private static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(
        this object settings,
        string settingsRoot)
    {
        return from property in settings
            .GetType()
            .GetProperties()
                let keyName = $"{settingsRoot}:{property.Name}"
                let keyValue = property.GetValue(settings)?.ToString()
                select new KeyValuePair<string, string>(keyName, keyValue!);
    }

    /// <summary>
    /// Adds user secrets to <see cref="IConfigurationBuilder"/> and tries to
    /// find these from the executing assembly by default.
    /// <remarks>
    /// Only works when the DEBUG preprocessor directive is defined.
    /// </remarks>
    /// </summary>
    /// <param name="configurationBuilder">An <see cref="IConfigurationBuilder"/>.</param>
    /// <returns>The input <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddUserSecretsFromAssembly(this IConfigurationBuilder configurationBuilder)
    {
#if DEBUG
        // It's only allowed to add user secrets in debug mode by design.
        // User secrets are for developers only. For production builds you
        // should use environment variables.
        return configurationBuilder.AddUserSecrets(Assembly.GetExecutingAssembly());
#endif
    }
}