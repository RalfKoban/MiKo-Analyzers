﻿using System.Collections.Generic;
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

        public static bool ContainsAny(this string value, params string[] prefixes) => ContainsAny(value, StringComparison.OrdinalIgnoreCase, prefixes);

        public static bool ContainsAny(this string value, StringComparison comparison, params string[] prefixes) => !string.IsNullOrEmpty(value) && prefixes.Any(_ => value.Contains(_, comparison));

        public static bool EqualsAny(this string value, params string[] phrases) => !string.IsNullOrEmpty(value) && EqualsAny(value, StringComparison.OrdinalIgnoreCase, phrases);

        public static bool EqualsAny(this string value, StringComparison comparison, params string[] phrases) => !string.IsNullOrEmpty(value) && phrases.Any(_ => value.Equals(_, comparison));

        public static bool StartsWithAny(this string value, params string[] prefixes) => StartsWithAny(value, StringComparison.OrdinalIgnoreCase, prefixes);

        public static bool StartsWithAny(this string value, StringComparison comparison, params string[] prefixes) => !string.IsNullOrEmpty(value) && prefixes.Any(_ => value.StartsWith(_, comparison));

        public static bool EndsWithAny(this string value, params string[] suffixes) => EndsWithAny(value, StringComparison.OrdinalIgnoreCase, suffixes);

        public static bool EndsWithAny(this string value, StringComparison comparison, params string[] suffixes) => !string.IsNullOrEmpty(value) && suffixes.Any(_ => value.EndsWith(_, comparison));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(this char value) => char.IsWhiteSpace(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperCase(this char value) => char.IsUpper(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this char value) => char.IsNumber(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith<T>(this IEnumerable<T> values) where T: class => string.Concat(values.Where(_ => _ != null));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith<T>(this IEnumerable<T> values, string separator) => string.Join(separator, values);

        public static string HumanizedConcatenated<T>(this IEnumerable<T> values)
        {
            var items = values.SurroundedWith('\'').ToList();

            const string Separator = ", ";
            const string SeparatorForLast = " or ";

            var count = items.Count;
            switch (count)
            {
                case 0: return string.Empty;
                case 1: return items[0];
                case 2: return items.ConcatenatedWith(SeparatorForLast);
                default: return string.Concat(items.Take(count - 1).ConcatenatedWith(Separator), SeparatorForLast, items.Last());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<string> SurroundedWith<T>(this IEnumerable<T> values, char surrounding) => values.Select(_ => string.Concat(surrounding, _, surrounding));

        internal static string HumanizedTakeFirst(this string value, int max)
        {
            var index = Math.Min(max, value.Length);
            return index <= 0 || index == value.Length
                       ? value
                       : value.Substring(0, index) + "...";
        }

        internal static bool IsEntityMarker(this string symbolName) => symbolName.EndsWithAny(Constants.Markers.Entities) && !symbolName.EndsWithAny(Constants.Markers.ViewModels);

        internal static bool HasEntityMarker(this string symbolName)
        {
            if (!symbolName.ContainsAny(Constants.Markers.Entities)) return false;
            if (symbolName.ContainsAny(Constants.Markers.ViewModels)) return false;
            if (symbolName.ContainsAny(Constants.Markers.SpecialModels)) return false;

            return true;

        }

        internal static bool HasCollectionMarker(this string symbolName) => symbolName.EndsWithAny(Constants.Markers.Collections);

        internal static string WithoutParaTags(this string value) => value.RemoveAll(Constants.ParaTags);

        internal static IEnumerable<string> WithoutParaTags(this IEnumerable<string> values) => values.Select(WithoutParaTags);

        internal static string Remove(this string value, string phrase) => value.Replace(phrase, string.Empty);

        internal static string RemoveAll(this string value, string[] values) => values.Aggregate(value, (current, s) => current.Remove(s));

        internal static string WithoutSuffix(this string value, string suffix)
        {
            var length = value.Length - suffix.Length;
            return length > 0 ? value.Substring(0, length) : string.Empty;
        }

        internal static string GetNameOnlyPart(this string fullName) => fullName.Substring(fullName.LastIndexOf('.') + 1);

        internal static HashSet<string> ToHashSet(this IEnumerable<string> source) => new HashSet<string>(source);
    }
}