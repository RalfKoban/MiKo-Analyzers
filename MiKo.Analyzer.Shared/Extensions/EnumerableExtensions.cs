using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System.Linq
{
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Removes the specified values from the <see cref="HashSet{T}"/> and returns the modified set.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the set.
        /// </typeparam>
        /// <param name="source">
        /// The set to modify.
        /// </param>
        /// <param name="values">
        /// The values to remove from the set.
        /// </param>
        /// <returns>
        /// The modified set after removing the specified values.
        /// </returns>
        internal static HashSet<T> Except<T>(this HashSet<T> source, IEnumerable<T> values) where T : class
        {
            source.ExceptWith(values);

            return source;
        }

        /// <summary>
        /// Provides a sequence that excludes the specified value from the source sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The source sequence.
        /// </param>
        /// <param name="value">
        /// The value to exclude from the sequence.
        /// </param>
        /// <returns>
        /// A sequence that contains all elements from the source sequence except the specified value.
        /// </returns>
        internal static IEnumerable<T> Except<T>(this IEnumerable<T> source, T value) where T : class
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in source)
            {
                if (ReferenceEquals(item, value) || Equals(item, value))
                {
                    continue;
                }

                yield return item;
            }
        }

        /// <summary>
        /// Removes the specified values from the <see cref="HashSet{T}"/> and returns the modified set.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the set.
        /// </typeparam>
        /// <param name="source">
        /// The set to modify.
        /// </param>
        /// <param name="values">
        /// The values to remove from the set.
        /// </param>
        /// <returns>
        /// The modified set after removing the specified values.
        /// </returns>
        internal static HashSet<T> Except<T>(this HashSet<T> source, params T[] values) where T : class
        {
            source.ExceptWith(values);

            return source;
        }

        /// <summary>
        /// Determines whether the specified array contains an element that matches the given condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <param name="value">
        /// The array to search.
        /// </param>
        /// <param name="match">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if an element that matches the condition is found; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Exists<T>(this T[] value, Predicate<T> match) => value.Length > 0 && Array.Exists(value, match);

        /// <summary>
        /// Determines whether the specified read-only list contains an element that matches the given condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the list.
        /// </typeparam>
        /// <param name="value">
        /// The read-only list to search.
        /// </param>
        /// <param name="match">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if an element that matches the condition is found; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Exists<T>(this IReadOnlyList<T> value, Predicate<T> match)
        {
            if (value.Count <= 0)
            {
                return false;
            }

            switch (value)
            {
                case List<T> list: return list.Exists(match);
                case T[] array: return array.Exists(match);

                default:
                    return value.Any(new Func<T, bool>(match));
            }
        }

        /// <summary>
        /// Determines whether the specified array contains an element that matches the given condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <param name="value">
        /// The array to search.
        /// </param>
        /// <param name="match">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The first element that matches the condition, or the default value if no such element is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T Find<T>(this T[] value, Predicate<T> match) => value.Length > 0 ? Array.Find(value, match) : default;

        /// <summary>
        /// Finds the index of the first occurrence of the specified value in the array.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <param name="source">
        /// The array to search.
        /// </param>
        /// <param name="value">
        /// The value to locate in the array.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the value, or -1 if the value is not found.
        /// </returns>
        internal static int IndexOf<T>(this T[] source, T value) => source.Length is 0 ? -1 : Array.IndexOf(source, value);

        /// <summary>
        /// Finds the index of the first element in the array that satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <param name="source">
        /// The array to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The zero-based index of the first element that satisfies the condition, or -1 if no such element is found.
        /// </returns>
        internal static int IndexOf<T>(this T[] source, Func<T, bool> predicate) => source.Length is 0 ? -1 : source.IndexOf(new Predicate<T>(predicate));

        /// <summary>
        /// Finds the index of the first element in the array that satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <param name="source">
        /// The array to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The zero-based index of the first element that satisfies the condition, or -1 if no such element is found.
        /// </returns>
        internal static int IndexOf<T>(this T[] source, Predicate<T> predicate) => source.Length is 0 ? -1 : Array.FindIndex(source, predicate);

        /// <summary>
        /// Finds the index of the first element in the sequence that satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The zero-based index of the first element that satisfies the condition, or -1 if no such element is found.
        /// </returns>
        internal static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            switch (source)
            {
                case T[] array: return array.IndexOf(predicate);
                case List<T> list: return list.FindIndex(new Predicate<T>(predicate));
            }

            var index = 0;

            foreach (var value in source)
            {
                if (predicate(value))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Determines whether the specified sequence is an empty array.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the sequence is an empty array; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsEmptyArray<T>(this IEnumerable<T> source) => source is T[] array && array.Length is 0;

        /// <summary>
        /// Determines whether the specified sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the sequence contains no elements; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool None<T>(this IEnumerable<T> source) => source.Any() is false;

        /// <summary>
        /// Determines whether the specified read-only collection contains no elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the collection.
        /// </typeparam>
        /// <param name="source">
        /// The read-only collection to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the collection contains no elements; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this IReadOnlyCollection<T> source) => source.Count is 0;

        /// <summary>
        /// Determines whether the specified sequence contains no elements that satisfy the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if no elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) => source.All(_ => predicate(_) is false);

        /// <summary>
        /// Determines whether the specified array contains no elements that satisfy the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <param name="source">
        /// The array to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if no elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool None<T>(this T[] source, Func<T, bool> predicate)
        {
            var length = source.Length;

            if (length is 0)
            {
                return true;
            }

            for (var index = 0; index < length; index++)
            {
                if (predicate(source[index]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="ReadOnlySpan{T}"/> contains no elements that satisfy the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the span.
        /// </typeparam>
        /// <param name="source">
        /// The span to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if no elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool None<T>(this in ReadOnlySpan<T> source, Func<T, bool> predicate)
        {
            var sourceLength = source.Length;

            if (sourceLength > 0)
            {
                for (var index = 0; index < sourceLength; index++)
                {
                    if (predicate(source[index]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified sequence contains more elements than the specified count.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence to evaluate.
        /// </param>
        /// <param name="count">
        /// The number of elements to compare against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the sequence contains more elements than the specified count; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool MoreThan<T>(this IEnumerable<T> source, in int count)
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
                        for (var index = 0; index <= count; index++)
                        {
                            if (enumerator.MoveNext() is false)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
            }
        }

        /// <summary>
        /// Converts the specified sequence to an array, using the specified comparer to order the elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence to convert.
        /// </param>
        /// <param name="comparer">
        /// The comparer to use for ordering the elements.
        /// </param>
        /// <returns>
        /// An array of all elements from the sequence, ordered according to the comparer.
        /// </returns>
        internal static T[] ToArray<T>(this IEnumerable<T> source, IComparer<T> comparer) => source.ToArray(_ => _, comparer);

        /// <summary>
        /// Converts the specified read-only list to an array of keys by applying the specified selector callback.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key returned by the selector.
        /// </typeparam>
        /// <typeparam name="TSource">
        /// The type of elements in the list.
        /// </typeparam>
        /// <param name="source">
        /// The read-only list to convert.
        /// </param>
        /// <param name="keySelector">
        /// A callback to extract a key from each element.
        /// </param>
        /// <returns>
        /// An array of keys extracted from each element in the list, or an empty array if the list contains no elements.
        /// </returns>
        internal static TKey[] ToArray<TKey, TSource>(this IReadOnlyList<TSource> source, Func<TSource, TKey> keySelector)
        {
            var sourceCount = source.Count;

            if (sourceCount is 0)
            {
                return Array.Empty<TKey>();
            }

            var result = new TKey[sourceCount];

            for (var index = 0; index < sourceCount; index++)
            {
                result[index] = keySelector(source[index]);
            }

            return result;
        }

        /// <summary>
        /// Converts the specified read-only collection to an array of keys by applying the specified selector callback.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key returned by the selector.
        /// </typeparam>
        /// <typeparam name="TSource">
        /// The type of elements in the collection.
        /// </typeparam>
        /// <param name="source">
        /// The read-only collection to convert.
        /// </param>
        /// <param name="keySelector">
        /// A callback to extract a key from each element.
        /// </param>
        /// <returns>
        /// An array of keys extracted from each element in the collection, or an empty array if the collection contains no elements.
        /// </returns>
        internal static TKey[] ToArray<TKey, TSource>(this IReadOnlyCollection<TSource> source, Func<TSource, TKey> keySelector)
        {
            var sourceCount = source.Count;

            if (sourceCount is 0)
            {
                return Array.Empty<TKey>();
            }

            var index = 0;
            var result = new TKey[sourceCount];

            foreach (var item in source)
            {
                result[index++] = keySelector(item);
            }

            return result;
        }

        /// <summary>
        /// Converts the specified sequence to an array of keys by applying the specified selector callback.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key returned by the selector.
        /// </typeparam>
        /// <typeparam name="TSource">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence to convert.
        /// </param>
        /// <param name="keySelector">
        /// A callback to extract a key from each element.
        /// </param>
        /// <returns>
        /// An array of keys extracted from each element in the sequence.
        /// </returns>
        internal static TKey[] ToArray<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            switch (source)
            {
                case TSource[] array: return ToArray(array, keySelector);
                case ImmutableArray<TSource> array: return Microsoft.CodeAnalysis.ImmutableArrayExtensions.ToArray(array, keySelector);
                case IReadOnlyList<TSource> list: return ToArray(list, keySelector);
                case IReadOnlyCollection<TSource> collection: return ToArray(collection, keySelector);
                default:
                    return source.Select(keySelector).ToArray();
            }
        }

        /// <summary>
        /// Converts the specified sequence to an array, ordering the elements by the specified key and comparer.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key used for ordering.
        /// </typeparam>
        /// <typeparam name="TSource">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence to convert.
        /// </param>
        /// <param name="keySelector">
        /// A callback to extract a key from each element for ordering.
        /// </param>
        /// <param name="comparer">
        /// The comparer to use for ordering the keys.
        /// </param>
        /// <returns>
        /// An array of all elements from the sequence, ordered by the specified key and comparer.
        /// </returns>
        internal static TSource[] ToArray<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) => source.OrderBy(keySelector, comparer).ToArray(); // ncrunch: no coverage

#if NETSTANDARD2_0

        /// <summary>
        /// Converts the elements of the specified sequence to a <see cref="HashSet{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence whose elements are to be converted.
        /// </param>
        /// <returns>
        /// A <see cref="HashSet{T}"/> that contains the elements from the sequence.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source); // ncrunch: no coverage

        /// <summary>
        /// Converts the elements of the specified sequence to a <see cref="HashSet{T}"/> using the specified equality comparer.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence whose elements are to be converted.
        /// </param>
        /// <param name="comparer">
        /// The equality comparer to use for the hash set.
        /// </param>
        /// <returns>
        /// A <see cref="HashSet{T}"/> that contains the elements from the sequence, compared using the specified comparer.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) => new HashSet<T>(source, comparer);

#endif

//// ncrunch: no coverage start

        /// <summary>
        /// Converts the elements of the specified sequence to a <see cref="HashSet{TResult}"/> by applying the specified selector callback to each element.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of elements in the source sequence.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of elements in the resulting hash set.
        /// </typeparam>
        /// <param name="source">
        /// The sequence whose elements are to be converted.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// A <see cref="HashSet{TResult}"/> that contains the transformed elements from the source sequence.
        /// </returns>
        internal static HashSet<TResult> ToHashSet<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            switch (source)
            {
                case TSource[] array: return array.ToHashSet(selector);
                case List<TSource> list: return list.ToHashSet(selector);
                case ImmutableArray<TSource> array: return Microsoft.CodeAnalysis.ImmutableArrayExtensions.ToHashSet(array, selector);
            }

            var result = new HashSet<TResult>();

            foreach (var item in source)
            {
                result.Add(selector(item));
            }

            return result;
        }

        /// <summary>
        /// Converts the elements of the specified list to a <see cref="HashSet{TResult}"/> by applying the specified selector callback to each element.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of elements in the source list.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of elements in the resulting hash set.
        /// </typeparam>
        /// <param name="source">
        /// The list whose elements are to be converted.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// A <see cref="HashSet{TResult}"/> that contains the transformed elements from the source list.
        /// </returns>
        internal static HashSet<TResult> ToHashSet<TSource, TResult>(this List<TSource> source, Func<TSource, TResult> selector)
        {
            var result = new HashSet<TResult>();
            var length = source.Count;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    result.Add(selector(source[index]));
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the elements of the specified array to a <see cref="HashSet{TResult}"/> by applying the specified selector callback to each element.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of elements in the source array.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of elements in the resulting hash set.
        /// </typeparam>
        /// <param name="source">
        /// The array whose elements are to be converted.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// A <see cref="HashSet{TResult}"/> that contains the transformed elements from the source array.
        /// </returns>
        internal static HashSet<TResult> ToHashSet<TSource, TResult>(this TSource[] source, Func<TSource, TResult> selector)
        {
            var result = new HashSet<TResult>();
            var length = source.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    result.Add(selector(source[index]));
                }
            }

            return result;
        }

        //// ncrunch: no coverage end

        /// <summary>
        /// Determines whether all elements in the specified array satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <param name="source">
        /// The array to evaluate.
        /// </param>
        /// <param name="match">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TrueForAll<T>(this T[] source, Predicate<T> match) => Array.TrueForAll(source, match);

        /// <summary>
        /// Filters the specified sequence and returns only the non-<see langword="null"/> elements from that sequence.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of elements in the sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence to filter.
        /// </param>
        /// <returns>
        /// A sequence that contains only the non-<see langword="null"/> elements from the source sequence.
        /// </returns>
        internal static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource> source) where TSource : class
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in source)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }
    }
}