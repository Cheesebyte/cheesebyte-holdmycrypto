using System.Text.Json.Serialization;
using Cheesebyte.HoldMyCrypto.Utils;

namespace Cheesebyte.HoldMyCrypto.Importers.Implementations.Bl3p.Models;

/// <summary>
/// BL3P Transaction DTO.
/// </summary>
/// <param name="Side">Either 'credit' or 'debit'.</param>
/// <param name="Type">Either 'fee' or 'trade'.</param>
public record Bl3pTransaction(
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("transaction_id")
    ] int Id,
    [property: JsonPropertyName("amount")] Bl3pAmount Amount,
    [property: JsonPropertyName("balance")] Bl3pAmount Balance,
    [property:
        JsonConverter(typeof(UnixToNullableDateTimeConverter)),
        JsonPropertyName("date")
    ] DateTime? Date,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("order_id")
    ] int OrderId,
    [property:
        JsonNumberHandling(JsonNumberHandling.AllowReadingFromString),
        JsonPropertyName("trade_id")
    ] int TradeId,
    [property: JsonPropertyName("debit_credit")] string Side,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("fee")] Bl3pAmount? Fee,
    [property: JsonPropertyName("contra_amount")] Bl3pAmount? ContraAmount,
    [property: JsonPropertyName("price")] Bl3pAmount? Price
);