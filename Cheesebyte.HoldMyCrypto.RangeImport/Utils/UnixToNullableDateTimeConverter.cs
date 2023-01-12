using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cheesebyte.HoldMyCrypto.Utils
{
    /// <summary>
    /// <para>
    /// Unix timestamp to DateTime converter for System.Text.Json
    /// </para>
    /// <para>
    /// Inspired by: https://stackoverflow.com/a/70225152
    /// </para>
    /// </summary>
    public class UnixToNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override bool HandleNull => true;
        public bool IsFormatInSeconds { get; set; }

        private readonly long _unixMinSeconds;
        private readonly long _unixMaxSeconds;

        public UnixToNullableDateTimeConverter()
        {
            var minValueSeconds = DateTimeOffset.MinValue.ToUnixTimeSeconds();
            var maxValueSeconds = DateTimeOffset.MaxValue.ToUnixTimeSeconds();
            var unixEpochSeconds = DateTimeOffset.UnixEpoch.ToUnixTimeSeconds();

            _unixMinSeconds = minValueSeconds - unixEpochSeconds; // -62_135_596_800
            _unixMaxSeconds = maxValueSeconds - unixEpochSeconds; // 253_402_300_799
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override DateTime? Read(
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
                    return FromTime(reader.GetInt64());
            }

            return reader.TryGetInt64(out var time) ?
                FromTime(time) :
                null;
        }
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Write(
            Utf8JsonWriter writer,
            DateTime? value,
            JsonSerializerOptions options) =>
            throw new NotSupportedException();

        private DateTime? HandleTokenString(string? tokenString)
        {
            return long.TryParse(tokenString, out var result) ?
                FromTime(result) :
                null;
        }

        private DateTime? FromTime(long time)
        {
            var isFormatInSeconds = IsFormatInSeconds &&
                time > _unixMinSeconds &&
                time < _unixMaxSeconds;

            // if 'IsFormatInSeconds' is unspecified, then deduce the correct
            // type based on whether it can be represented in the allowed .NET
            // DateTime range.
            return isFormatInSeconds ?
                DateTimeOffset.FromUnixTimeSeconds(time).UtcDateTime :
                DateTimeOffset.FromUnixTimeMilliseconds(time).UtcDateTime;
        }
    }
}
