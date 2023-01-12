using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cheesebyte.HoldMyCrypto.Utils;

/// <summary>
/// Int64 (long) to Decimal converter for System.Text.Json
/// </summary>
public class DecimalConverter : JsonConverter<decimal>
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override decimal Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                var tokenString = reader.GetString();
                return HandleTokenString(tokenString);
            case JsonTokenType.Number:
                return reader.GetDecimal();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private decimal HandleTokenString(string? tokenString)
    {
        // Allow parsing of numbers with exponent to satisfy the
        // models for certain exchanges (e.g. like Bybit, which
        // uses normal notation and exponential notation in an
        // alternated fashion).
        //
        // Use invariant to handle both English and Dutch cultures
        // at once. Should be relatively safe. If this program
        // ever were to implement full and correct usage of
        // culture settings, the use of this invariant culture
        // should be reworked.
        return decimal.TryParse(
            tokenString,
            // NumberStyles.AllowExponent,
            CultureInfo.InvariantCulture,
            out var result)
            ? result
            : 0m;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Write(
        Utf8JsonWriter writer,
        decimal value,
        JsonSerializerOptions options) =>
        writer.WriteNumberValue(value);
}