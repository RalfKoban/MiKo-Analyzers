using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System
{
    internal static class StringExtensions
    {
        public static bool StartsWithAny(this string value, params string[] prefixes)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (value == string.Empty) return false;


            return prefixes.Any(prefix => value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Concatenated<T>(this IEnumerable<T> values, string separator = "") => string.Join(separator, values);
    }
}