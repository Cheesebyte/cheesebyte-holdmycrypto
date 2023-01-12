using System.Diagnostics;
using System.Text;
using FluentValidation;

namespace Cheesebyte.HoldMyCrypto.Console.Extensions;

/// <summary>
/// Extension methods for extending <see cref="Console"/> functionality.
/// </summary>
public static class ConsoleExtensions
{
    /// <summary>
    /// Prints a horizontal divider to the standard output stream.
    /// </summary>
    /// <param name="dividerColumns">
    /// The number of columns to use for the horizontal divider.
    /// </param>
    public static void WriteDivider(int dividerColumns = 60)
    {
        var line = string.Join(string.Empty, Enumerable.Repeat('-', dividerColumns));

        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.WriteLine(line);
        System.Console.ResetColor();
    }

    /// <summary>
    /// Prints FluentValidation <see cref="ValidationException"/> with
    /// user-readable formatting to the standard output stream.
    /// </summary>
    /// <param name="exception">
    /// A FluentValidation <see cref="ValidationException"/>.
    /// </param>
    public static void WriteToConsole(this ValidationException exception)
    {
        System.Console.WriteLine();
        WriteDivider();

        System.Console.WriteLine($" Found {exception.Errors.Count()} error(s) in your options:");
        WriteDivider();

        foreach (var error in exception.Errors)
        {
            System.Console.Write($"> ");
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.Write(error.PropertyName);
            System.Console.ResetColor();
            System.Console.Write($":\t");
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Write(error.ErrorMessage);
            System.Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.WriteLine($"\t(current: '{error.AttemptedValue}'): ");
            System.Console.ResetColor();
        }

        System.Console.ResetColor();
    }

    /// <summary>
    /// <para>
    /// Formats <paramref name="values"/> into a user-readable table and
    /// returns it for printing or saving to a custom output stream.
    /// </para>
    /// <para>
    /// https://stackoverflow.com/a/19353995
    /// </para>
    /// </summary>
    /// <param name="values">
    /// Value input enumerable to work on.
    /// </param>
    /// <param name="columnHeaders">
    /// Header titles to use for each column in the table.
    /// </param>
    /// <param name="valueSelectors">
    /// Function to use per item in the column list to retrieve
    /// the final user-readable value to print.
    /// </param>
    public static string ToStringTable<T>(
        this IEnumerable<T> values,
        IReadOnlyList<string> columnHeaders,
        params Func<T, object>[] valueSelectors)
    {
        return ToStringTable(values.ToArray(), columnHeaders, valueSelectors);
    }

    private static string ToStringTable<T>(
        this IReadOnlyList<T> values,
        IReadOnlyList<string> columnHeaders,
        params Func<T, object>[] valueSelectors)
    {
        Debug.Assert(columnHeaders.Count == valueSelectors.Length);
        var arrValues = new string[values.Count + 1, valueSelectors.Length];

        // Fill headers
        for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
        {
            arrValues[0, colIndex] = columnHeaders[colIndex];
        }

        // Fill table rows
        for (int rowIndex = 1; rowIndex < arrValues.GetLength(0); rowIndex++)
        {
            for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
            {
                arrValues[rowIndex, colIndex] = valueSelectors[colIndex]
                    .Invoke(values[rowIndex - 1])
                    .ToString() ?? string.Empty;
            }
        }

        return ToStringTable(arrValues);
    }

    private static string ToStringTable(this string[,] arrValues)
    {
        var maxColumnsWidth = GetMaxColumnsWidth(arrValues);
        var headerSpliter = new string('-', maxColumnsWidth.Sum(i => i + 3) - 1);

        var sb = new StringBuilder();
        for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
        {
            for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
            {
                // Print cell
                string cell = arrValues[rowIndex, colIndex];
                cell = cell.PadRight(maxColumnsWidth[colIndex]);
                sb.Append(" | ");
                sb.Append(cell);
            }

            // Print end of line
            sb.Append(" | ");
            sb.AppendLine();

            // Print splitter
            if (rowIndex != 0) continue;
            sb.Append($" |{headerSpliter}| ");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static int[] GetMaxColumnsWidth(string[,] arrValues)
    {
        var maxColumnsWidth = new int[arrValues.GetLength(1)];
        for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
        {
            for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
            {
                int newLength = arrValues[rowIndex, colIndex].Length;
                int oldLength = maxColumnsWidth[colIndex];

                if (newLength > oldLength)
                {
                    maxColumnsWidth[colIndex] = newLength;
                }
            }
        }

        return maxColumnsWidth;
    }
}