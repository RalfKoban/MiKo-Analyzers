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
        /// Splits the text into substrings based on the specified string separators using the provided comparison and split options.
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
        }

        /// <summary>
        /// Splits the text into substrings based on the specified string separators using the provided comparison and split options.
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
        /// If the input string is <see langword="null"/>, the method returns an empty array.
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