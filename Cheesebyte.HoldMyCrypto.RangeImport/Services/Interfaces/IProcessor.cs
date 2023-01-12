using Cheesebyte.HoldMyCrypto.Models;

namespace Cheesebyte.HoldMyCrypto.Services.Interfaces;

/// <summary>
/// General purpose processing object for transactional info. Does not
/// take into account batched data sets yet and is considered for
/// support at a later date.
/// </summary>
public interface IProcessor
{
    /// <summary>
    /// <para>
    /// Enriches transaction info. If data is not complete, the
    /// processor will try to add it.
    /// </para>
    /// <para>
    /// This is required because some importers cannot load all required
    /// data due to limitations in REST APIs or backend systems used.
    /// </para>
    /// <para>
    /// Essentially, it allows transaction properties to be normalised
    /// for further and more complex use-cases.
    /// </para>
    /// </summary>
    /// <param name="allTransactions"></param>
    /// <returns>
    /// A new list with mutated transactions. Original data
    /// should be treated by any implementation to be immutable.
    /// </returns>
    IEnumerable<ExchangeTransaction> Process(ICollection<ExchangeTransaction> allTransactions);
}