using Cheesebyte.HoldMyCrypto.Enums;

namespace Cheesebyte.HoldMyCrypto.Models;

/// <summary>
/// Transactions work like in the typical double entry system that has been
/// used in the 'normal' financial world for a long time. For an example of
/// how this works, see: https://stackoverflow.com/a/4074431
/// </summary>
/// <param name="TransactionId">
/// Externally recognisable transaction ID (e.g. for Bitcoin, it's
/// the public tx ID).
/// </param>
/// <param name="Network">
/// What network (e.g. Bitcoin, a bank account, an rangeImporter) this
/// transaction came from.
/// </param>
/// <param name="Type">
/// What side of an transaction is represented (e.g. credit, debit or transfer).
/// </param>
/// <param name="Amount">
/// What amount of asset value is represented by this transaction.
/// </param>
/// <param name="Category">
/// Double entry-type of category associated with this transaction.
/// </param>
public record ExchangeTransaction(
    DateTime Timestamp,
    string SourceName,
    string TransactionId,
    ExchangeAmount Amount,
    ExchangeNetwork Network,
    ExchangeCategory Category,
    ExchangeTransactionType Type) : BaseExchangeTimeData(Timestamp, SourceName)
{
    public override string ToString()
    {
        var category = Category.ToString().PadRight(10);
        var timestamp = Timestamp.ToShortTimeString();
        var amount = Amount.ToString().PadRight(18);
        var source = SourceName.PadRight(10);
        var type = Type.ToString().PadRight(8);

        return $"[{category}]: {amount} @ {timestamp} > {type} > {source} > {TransactionId}";
    }
}