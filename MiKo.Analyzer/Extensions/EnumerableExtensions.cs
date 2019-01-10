using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    internal static class EnumerableExtensions
    {
        public static bool None<T>(this IEnumerable<T> source) => !source.Any();
    }
}