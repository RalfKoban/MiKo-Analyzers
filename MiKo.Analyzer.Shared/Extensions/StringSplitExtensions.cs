using System;
using System.Collections.Generic;
using System.Linq;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    internal static class StringSplitExtensions
    {
        public static SplitReadOnlySpanEnumerator SplitBy(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> separatorChars) => SplitBy(value, separatorChars, StringSplitOptions.None);

        public static SplitReadOnlySpanEnumerator SplitBy(this in ReadOnlySpan<char> value, in ReadOnlySpan<char> separatorChars, in StringSplitOptions options) => new SplitReadOnlySpanEnumerator(value, separatorChars, options);

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