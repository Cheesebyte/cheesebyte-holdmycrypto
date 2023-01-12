namespace Cheesebyte.HoldMyCrypto.Enums;

/// <summary>
/// Defines how an amount of worth is moved between transactions.
/// </summary>
public enum ExchangeTransactionType
{
    Credit,    // Added value
    Debit,     // Removed value
    Transfer   // Moved value
}