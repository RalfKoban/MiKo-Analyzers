using System;
using System.Collections.Generic;
using System.Linq;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="ReadOnlySpan{T}"/>s that allow to split them into different parts.
    /// </summary>
    internal static class StringSplitExtensions
    {
        /// <summary>
        /// Splits the text into substrings based on the specified separator characters.
        /// </summary>
        /// <param name="value">
        /// The text to split.
        /// </param>
        /// <param name="separatorChars">
        /// The characters that delimit the substrings in the text.
        /// </param>
        /// <returns>
        /// A <see cref="SplitReadOnlySpanEnumerator"/> that allows enumeration over the split substrings.
        /// </returns>
        public static SplitReadOnlySpanEnumerator SplitBy(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> separatorChars) => SplitBy(value, separatorChars, StringSplitOptions.None);

        /// <summary>
        /// Splits the text into substrings based on the specified separator characters and split options.
        /// </summary>
        /// <param name="value">
        /// The text to split.
        /// </param>
        /// <param name="separatorChars">
        /// The characters that delimit the substrings in the text.
        /// </param>
        /// <param name="options">
        /// A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.
        /// </param>
        /// <returns>
        /// A <see cref="SplitReadOnlySpanEnumerator"/> that allows enumeration over the split substrings.
        /// </returns>
        public static SplitReadOnlySpanEnumerator SplitBy(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> separatorChars, in StringSplitOptions options) => new SplitReadOnlySpanEnumerator(value, separatorChars, options);

        /// <summary>
        /// Splits the text into substrings based on the specified <see cref="string"/> separators using the provided comparison and split options.
        /// </summary>
        /// <param name="value">
        /// The text to split.
        /// </param>
        /// <param name="findings">
        /// The strings that delimit the substrings in the text.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration values that specifies the rules for the search.
        /// The default is <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </param>
        /// <param name="options">
        /// A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.
        /// The default is <see cref="StringSplitOptions.None"/>.
        /// </param>
        /// <returns>
        /// A collection of the substrings from the text that are delimited by one or more strings from the separator collection.
        /// If no delimiters are found, the method returns an empty array or the original text.
        /// </returns>
        public static IReadOnlyList<string> SplitBy(this in ReadOnlySpan<char> value, in ReadOnlySpan<string> findings, in StringComparison comparison = StringComparison.OrdinalIgnoreCase, in StringSplitOptions options = StringSplitOptions.None)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<string>();
            }

            var substringIndices = new List<(int, string)>();

            for (int findingsIndex = 0, length = findings.Length; findingsIndex < length; findingsIndex++)
            {
                var finding = findings[findingsIndex];

                var indices = value.AllIndicesOf(finding, comparison);
                var indicesLength = indices.Length;

                substringIndices.Capacity += indicesLength;

                for (var i = 0; i < indicesLength; i++)
                {
                    var index = indices[i];

                    substringIndices.Add((index, finding));
                }
            }

            if (substringIndices.Count is 0)
            {
                // happens in case we could not split the string
                return new[] { value.ToString() };
            }

            RemoveOverlaps(substringIndices);

            var results = new List<string>((substringIndices.Count * 2) + 1);

            var remainingString = value;

            // get substrings by tuple indices and remember all parts (in reverse order)
            foreach (var (index, finding) in substringIndices.OrderByDescending(_ => _.Item1))
            {
                var lastPart = remainingString.Slice(index + finding.Length).ToString();

                results.Add(lastPart);

                if (options != StringSplitOptions.RemoveEmptyEntries)
                {
                    results.Add(finding);
                }

                remainingString = remainingString.Slice(0, index);
            }

            // add first part of string as it would miss otherwise
            results.Add(remainingString.ToString());

            // ensure the correct order as the substrings were added in reverse order
            results.Reverse();

            return results;

            void RemoveOverlaps(List<(int, string)> indices)
            {
                var occupiedPositions = new HashSet<int>();

                var indicesInReverseOrder = indices.OrderByDescending(_ => _.Item2.Length)
                                                   .ThenByDescending(_ => _.Item1);

                // Filter duplicates that have the same index, but are shorter, to only include the long ones
                foreach (var (startPosition, foundText) in indicesInReverseOrder)
                {
                    // Note: do not mark the start position as occupied as we need that for the check to detect whether others occupy the same space
                    for (int i = 1, length = foundText.Length; i < length; i++)
                    {
                        occupiedPositions.Add(startPosition + i);
                    }
                }

                // filter the parts that are overlapped by others but occupy different positions (such as '0' inside '101')
                indices.RemoveAll(_ => occupiedPositions.Contains(_.Item1));

                // filter the parts that start with the same position (such as '1' at the start of '123')
                foreach (var group in indices.GroupBy(_ => _.Item1).Where(_ => _.Count() > 1))
                {
                    foreach (var overlap in group.OrderByDescending(_ => _.Item2.Length).Skip(1))
                    {
                        indices.Remove(overlap);
                    }
                }
            }
        }

        /// <summary>
        /// Splits the text into substrings based on the specified <see cref="string"/> separators using the provided comparison and split options.
        /// </summary>
        /// <param name="value">
        /// The text to split.
        /// </param>
        /// <param name="findings">
        /// The strings that delimit the substrings in the text.
        /// </param>
        /// <param name="comparison">
        /// One of the enumeration values that specifies the rules for the search.
        /// The default is <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </param>
        /// <param name="options">
        /// A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.
        /// The default is <see cref="StringSplitOptions.None"/>.
        /// </param>
        /// <returns>
        /// A collection of the substrings from the text that are delimited by one or more strings from the separator collection.
        /// If the input <see cref="string"/> is <see langword="null"/>, the method returns an empty array.
        /// </returns>
        public static IReadOnlyList<string> SplitBy(this string value, in ReadOnlySpan<string> findings, in StringComparison comparison = StringComparison.OrdinalIgnoreCase, in StringSplitOptions options = StringSplitOptions.None)
        {
            if (value is null)
            {
                return Array.Empty<string>();
            }

            return SplitBy(value.AsSpan(), findings, comparison, options);
        }
    }
}