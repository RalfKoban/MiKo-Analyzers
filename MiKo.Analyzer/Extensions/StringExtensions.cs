﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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

        public static bool ContainsAny(this string value, params string[] prefixes) => !string.IsNullOrEmpty(value) && prefixes.Any(value.Contains);

        public static bool StartsWithAny(this string value, StringComparison comparison, params string[] prefixes) => !string.IsNullOrEmpty(value) && prefixes.Any(_ => value.StartsWith(_, comparison));

        public static bool EndsWithAny(this string value, StringComparison comparison, params string[] suffixes) => !string.IsNullOrEmpty(value) && suffixes.Any(_ => value.EndsWith(_, comparison));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Concatenated<T>(this IEnumerable<T> values, string separator = "") => string.Join(separator, values);

        internal static bool IsEntityMarker(this string symbolName) => symbolName.EndsWith("Model", StringComparison.Ordinal) && !symbolName.EndsWith("ViewModel", StringComparison.Ordinal);

        internal static string WithoutParaTags(this string value) => value.Replace("<para>", string.Empty).Replace("<para />", string.Empty).Replace("</para>", string.Empty);

        internal static IEnumerable<string> WithoutParaTags(this IEnumerable<string> values) => values.Select(WithoutParaTags);
    }
}