namespace Cheesebyte.HoldMyCrypto.Extensions;

/// <summary>
/// Extension methods for <see cref="ICollection{T}"/>
/// and <see cref="IEnumerable{T}"/>.
/// </summary>
static class CollectionExtensions
{
    /// <summary>
    /// Creates all possible combinations of items from the given list.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="count"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Every possible combination of the input items.</returns>
    /// <exception cref="InvalidOperationException">Caught an invalid operation.</exception>
    public static IEnumerable<IEnumerable<T>> GetCombinations<T>(
        this IEnumerable<T> c,
        int count)
    {
        var collection = c.ToList();
        var listCount = collection.Count;

        if (count > listCount)
        {
            var errorMessage = $"{nameof(count)} is greater than the collection elements.";
            throw new InvalidOperationException(errorMessage);
        }

        var indices = Enumerable.Range(0, count).ToArray();
        do
        {
            yield return indices.Select(i => collection[i]).ToList();
            SetIndices(indices, indices.Length - 1, listCount);
        }
        while (!AllPlacesChecked(indices, listCount));
    }

    private static void SetIndices(IList<int> indices, int lastIndex, int count)
    {
        indices[lastIndex]++;
        if (lastIndex <= 0 || indices[lastIndex] != count)
        {
            return;
        }

        // Recurse until no indices left
        SetIndices(indices, lastIndex - 1, count - 1);
        indices[lastIndex] = indices[lastIndex - 1] + 1;
    }

    private static bool AllPlacesChecked(IReadOnlyList<int> indices, int places)
    {
        for (int i = indices.Count - 1; i >= 0; i--)
        {
            if (indices[i] != places)
            {
                return false;
            }

            places--;
        }

        return true;
    }
}