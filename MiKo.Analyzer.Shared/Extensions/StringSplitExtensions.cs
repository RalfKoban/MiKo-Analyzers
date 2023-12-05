using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringSplitExtensions
    {
        public static IEnumerable<string> SplitBy(this string value, string[] findings, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<string>();
            }

            return SplitBy(value.AsSpan(), findings, comparison);
        }

        public static IEnumerable<string> SplitBy(this ReadOnlySpan<char> value, string[] findings, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<string>();
            }

            var tuples = new List<(int, string)>();

            foreach (var finding in findings)
            {
                var indices = value.AllIndicesOf(finding, comparison);

                tuples.Capacity += indices.Count;

                foreach (var index in indices)
                {
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

        public static SplitReadOnlySpanEnumerator SplitBy(this string value, char separatorChar, StringSplitOptions options) => SplitBy(value.AsSpan(), new[] { separatorChar }, options);

        public static SplitReadOnlySpanEnumerator SplitBy(this string value, char[] separatorChars, StringSplitOptions options) => SplitBy(value.AsSpan(), separatorChars, options);

        public static SplitReadOnlySpanEnumerator SplitBy(this ReadOnlySpan<char> value, char[] separatorChars, StringSplitOptions options) => new SplitReadOnlySpanEnumerator(value, separatorChars, options);

        public static SplitReadOnlySpanEnumerator SplitBy(this string value, params char[] separatorChars) => SplitBy(value.AsSpan(), separatorChars, StringSplitOptions.None);

        public static SplitReadOnlySpanEnumerator SplitBy(this ReadOnlySpan<char> value, params char[] separatorChars) => SplitBy(value, separatorChars, StringSplitOptions.None);
    }
}