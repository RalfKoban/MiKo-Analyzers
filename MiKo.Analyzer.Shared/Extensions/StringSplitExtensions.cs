using System.Collections.Generic;
using System.Linq;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System
{
    internal static class StringSplitExtensions
    {
        public static SplitReadOnlySpanEnumerator SplitBy(this ReadOnlySpan<char> value, ReadOnlySpan<char> separatorChars) => SplitBy(value, separatorChars, StringSplitOptions.None);

        public static IReadOnlyList<string> SplitBy(this ReadOnlySpan<char> value, string[] findings, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<string>();
            }

            var tuples = new List<(int, string)>();

            var findingsLength = findings.Length;

            for (var findingsIndex = 0; findingsIndex < findingsLength; findingsIndex++)
            {
                var finding = findings[findingsIndex];

                var indices = value.AllIndicesOf(finding, comparison);

                tuples.Capacity += indices.Count;

                var indicesCount = indices.Count;

                for (var i = 0; i < indicesCount; i++)
                {
                    var index = indices[i];

                    tuples.Add((index, finding));
                }
            }

            var results = new List<string>((tuples.Count * 2) + 1);

            var remainingString = value;

            // get substrings by tuple indices and remember all parts (in reverse order)
            foreach (var (index, finding) in tuples.OrderByDescending(_ => _.Item1))
            {
                var lastPart = remainingString.Slice(index + finding.Length).ToString();

                results.Add(lastPart);
                results.Add(finding);

                remainingString = remainingString.Slice(0, index);
            }

            // add first part of string as it would miss otherwise
            results.Add(remainingString.ToString());

            // ensure the correct order as the substrings were added in reverse order
            results.Reverse();

            return results;
        }

        public static SplitReadOnlySpanEnumerator SplitBy(this ReadOnlySpan<char> value, ReadOnlySpan<char> separatorChars, StringSplitOptions options) => new SplitReadOnlySpanEnumerator(value, separatorChars, options);

        public static SplitReadOnlySpanEnumerator SplitBy(this ReadOnlySpan<char> value, char[] separatorChars, StringSplitOptions options) => SplitBy(value, separatorChars.AsSpan(), options);

        public static IReadOnlyList<string> SplitBy(this string value, string[] findings, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value is null)
            {
                return Array.Empty<string>();
            }

            return SplitBy(value.AsSpan(), findings, comparison);
        }
    }
}