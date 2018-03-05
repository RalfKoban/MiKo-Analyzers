using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using MiKoSolutions.Analyzers;

// ReSharper disable once CheckNamespace
namespace System
{
    internal static class StringExtensions
    {
        public static bool StartsWithAnyChar(this string value, string characters)
        {
            if (string.IsNullOrEmpty(value)) return false;

            var character = value[0];

            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < characters.Length; index++)
            {
                if (character == characters[index]) return true;
            }

            return false;
        }

        public static bool Contains(this string value, string finding, StringComparison comparison) => value.IndexOf(finding, comparison) >= 0;

        public static bool ContainsAny(this string value, params string[] prefixes) => !string.IsNullOrEmpty(value) && prefixes.Any(_ => value.Contains(_, StringComparison.OrdinalIgnoreCase));

        public static bool EqualsAny(this string value, StringComparison comparison, params string[] phrases) => !string.IsNullOrEmpty(value) && phrases.Any(_ => value.Equals(_, comparison));

        public static bool StartsWithAny(this string value, StringComparison comparison, params string[] prefixes) => !string.IsNullOrEmpty(value) && prefixes.Any(_ => value.StartsWith(_, comparison));

        public static bool EndsWithAny(this string value, StringComparison comparison, params string[] suffixes) => !string.IsNullOrEmpty(value) && suffixes.Any(_ => value.EndsWith(_, comparison));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(this char value) => char.IsWhiteSpace(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperCase(this char value) => char.IsUpper(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith<T>(this IEnumerable<T> values, string separator = "") => string.Join(separator, values);

        internal static bool IsEntityMarker(this string symbolName) => symbolName.EndsWithAny(StringComparison.OrdinalIgnoreCase, Constants.EntityMarkers) && !symbolName.EndsWithAny(StringComparison.OrdinalIgnoreCase, Constants.ViewModelMarkers);

        internal static bool HasEntityMarker(this string symbolName)
        {
            if (!symbolName.ContainsAny(Constants.EntityMarkers)) return false;
            if (symbolName.ContainsAny(Constants.ViewModelMarkers)) return false;
            if (symbolName.ContainsAny(Constants.SpecialModelMarkers)) return false;

            return true;

        }

        internal static bool HasCollectionMarker(this string symbolName) => symbolName.EndsWithAny(StringComparison.OrdinalIgnoreCase, Constants.CollectionMarkers);

        internal static string WithoutParaTags(this string value) => value.RemoveAll("<para>", "<para />", "<para/>", "</para>");

        internal static IEnumerable<string> WithoutParaTags(this IEnumerable<string> values) => values.Select(WithoutParaTags);

        internal static string RemoveAll(this string value, params string[] values) => values.Aggregate(value, (current, s) => current.Replace(s, string.Empty));

        internal static int IndexOfTimes(this string value, int times, char c)
        {
            var occurrences = Math.Min(value.Count(_ => _ == c), times);
            var index = -1;
            for (var i = 0; i < occurrences; i++)
            {
                index = value.IndexOf(c, ++index);
            }

            return index;
        }
    }
}