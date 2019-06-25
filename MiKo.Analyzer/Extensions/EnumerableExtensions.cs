using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    internal static class EnumerableExtensions
    {
        public static bool None<T>(this IEnumerable<T> source) => source.Any() is false;

        public static bool MoreThan<T>(this IEnumerable<T> source, int count)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));

                case ICollection<T> c:
                    return c.Count > count;

                case IReadOnlyCollection<T> c:
                    return c.Count > count;

                default:

                    using (var enumerator = source.GetEnumerator())
                    {
                        for (var i = 0; i <= count; i++)
                        {
                            if (enumerator.MoveNext() is false)
                                return false;
                        }

                        return true;
                    }
            }
        }
    }
}