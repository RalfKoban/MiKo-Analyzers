using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="ImmutableArray{T}"/>s.
    /// </summary>
    internal static class ImmutableArrayExtensions
    {
        /// <summary>
        /// Concatenates two <see cref="ImmutableArray{T}"/> sequences into a single sequence.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of elements in the arrays.
        /// </typeparam>
        /// <param name="first">
        /// The first sequence to concatenate.
        /// </param>
        /// <param name="second">
        /// The sequence to concatenate to the first sequence.
        /// </param>
        /// <returns>
        /// A sequence that contains the concatenated elements of the two input sequences.
        /// </returns>
        internal static IEnumerable<TSource> Concat<TSource>(this ImmutableArray<TSource> first, ImmutableArray<TSource> second)
        {
            if (first.Length > 0)
            {
                foreach (var item in first)
                {
                    yield return item;
                }
            }

            if (second.Length > 0)
            {
                foreach (var item in second)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="ImmutableArray{TSource}"/> contains a specific value using the provided equality comparer.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of elements in the array.
        /// </typeparam>
        /// <typeparam name="TTarget">
        /// The type of the value to locate in the array.
        /// </typeparam>
        /// <param name="source">
        /// The array to search.
        /// </param>
        /// <param name="value">
        /// The value to locate in the array.
        /// </param>
        /// <param name="comparer">
        /// The equality comparer to use for the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the array contains the specified value; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Contains<TSource, TTarget>(this in ImmutableArray<TSource> source, TTarget value, SymbolEqualityComparer comparer) where TSource : class, ISymbol
                                                                                                                                                where TTarget : class, ISymbol
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceLength = source.Length;

            if (sourceLength > 0)
            {
                for (var index = 0; index < sourceLength; index++)
                {
                    if (comparer.Equals(source[index], value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Counts the number of elements in the specified <see cref="ImmutableArray{T}"/> that satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <param name="source">
        /// The array to evaluate.
        /// </param>
        /// <param name="filter">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The number of elements that satisfy the condition.
        /// </returns>
        internal static int Count<T>(this in ImmutableArray<T> source, Func<T, bool> filter) where T : class, ISymbol
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceLength = source.Length;

            if (sourceLength <= 0)
            {
                return 0;
            }

            var count = 0;

            for (var index = 0; index < sourceLength; index++)
            {
                if (filter(source[index]))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Determines whether the specified <see cref="ImmutableArray{T}"/> contains no elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <param name="source">
        /// The array to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the array contains no elements; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this in ImmutableArray<T> source) => source.Length is 0;

        /// <summary>
        /// Projects each element of an immutable array to an <see cref="IEnumerable{T}"/> and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of symbols in the source array.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the elements of the resulting sequence.
        /// </typeparam>
        /// <param name="source">
        /// The immutable array of symbols to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each symbol.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the one-to-many transformation on each element of the source array.
        /// </returns>
        internal static IEnumerable<TResult> SelectMany<T, TResult>(this ImmutableArray<T> source, Func<T, IEnumerable<TResult>> selector) where T : ISymbol
                                                                                                                                           where TResult : class
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceLength = source.Length;

            if (sourceLength > 0)
            {
                for (var index = 0; index < sourceLength; index++)
                {
                    var value = source[index];

                    foreach (var item in selector(value))
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Projects each element of a sequence of symbols into an immutable array of symbols and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of symbols in the source sequence.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of symbols in the resulting sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence of symbols to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each symbol.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the one-to-many transformation on each symbol of the source list and flattening the results.
        /// </returns>
        internal static IEnumerable<TResult> SelectMany<T, TResult>(this IEnumerable<T> source, Func<T, ImmutableArray<TResult>> selector) where T : ISymbol
                                                                                                                                           where TResult : ISymbol
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery, so there is no need for a Select clause
            foreach (var value in source)
            {
                var array = selector(value);

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var arrayLength = array.Length;

                if (arrayLength > 0)
                {
                    for (var index = 0; index < arrayLength; index++)
                    {
                        yield return array[index];
                    }
                }
            }
        }

        /// <summary>
        /// Converts the elements of the specified <see cref="ImmutableArray{TSource}"/> to an array by applying the specified selector callback to each element.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of elements in the resulting array.
        /// </typeparam>
        /// <typeparam name="TSource">
        /// The type of elements in the source immutable array.
        /// </typeparam>
        /// <param name="source">
        /// The immutable array whose elements are to be converted.
        /// </param>
        /// <param name="keySelector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// An array of <typeparamref name="TKey"/> that contains the transformed elements from the source immutable array.
        /// </returns>
        internal static TKey[] ToArray<TKey, TSource>(this in ImmutableArray<TSource> source, Func<TSource, TKey> keySelector)
        {
            var length = source.Length;

            if (length is 0)
            {
                return Array.Empty<TKey>();
            }

            var result = new TKey[length];

            for (var index = 0; index < length; index++)
            {
                result[index] = keySelector(source[index]);
            }

            return result;
        }

        /// <summary>
        /// Converts the elements of the specified <see cref="ImmutableArray{TSource}"/> to a <see cref="HashSet{TResult}"/> by applying the specified selector callback to each element.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of elements in the source immutable array.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of elements in the resulting hash set.
        /// </typeparam>
        /// <param name="source">
        /// The immutable array whose elements are to be converted.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// A <see cref="HashSet{TResult}"/> that contains the transformed elements from the source immutable array.
        /// </returns>
        internal static HashSet<TResult> ToHashSet<TSource, TResult>(this in ImmutableArray<TSource> source, Func<TSource, TResult> selector)
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
    }
}