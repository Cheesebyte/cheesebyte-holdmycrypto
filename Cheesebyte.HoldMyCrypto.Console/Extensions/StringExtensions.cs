namespace Cheesebyte.HoldMyCrypto.Console.Extensions;

/// <summary>
/// Extension methods for modifying strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// <para>
    /// Adds a suffix to a text string. Defaults to ellipsis.
    /// </para>
    /// <para>
    /// https://stackoverflow.com/a/2776689
    /// </para>
    /// </summary>
    public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…")
    {
        return value?.Length > maxLength
            ? value.Substring(0, maxLength) + truncationSuffix
            : value;
    }
}