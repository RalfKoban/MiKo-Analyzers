using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis.CSharp;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace Microsoft.CodeAnalysis
{
    public static class SyntaxListExtensions
    {
        /// <summary>
        /// Determines whether all elements in the <see cref="SyntaxList{T}"/> satisfy the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="value">
        /// The list of syntax nodes to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool All<T>(this in SyntaxList<T> value, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            if (valueCount > 0)
            {
                for (var index = 0; index < valueCount; index++)
                {
                    if (predicate(value[index]) is false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether any element in the <see cref="SyntaxList{T}"/> satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="value">
        /// The list of syntax nodes to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any element satisfies the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Any<T>(this in SyntaxList<T> value, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            if (valueCount > 0)
            {
                for (var index = 0; index < valueCount; index++)
                {
                    if (predicate(value[index]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Provides a sequence that excludes the specified value from the <see cref="SyntaxList{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The source list.
        /// </param>
        /// <param name="value">
        /// The value to exclude from the list.
        /// </param>
        /// <returns>
        /// A sequence that contains all elements from the source sequence except the specified value.
        /// </returns>
        internal static IEnumerable<T> Except<T>(this SyntaxList<T> source, T value) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount > 0)
            {
                for (var index = 0; index < sourceCount; index++)
                {
                    var item = source[index];

                    if (ReferenceEquals(item, value) || Equals(item, value))
                    {
                        continue;
                    }

                    yield return item;
                }
            }
        }

        /// <summary>
        /// Finds the first element in the <see cref="SyntaxList{T}"/> that satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The first element that satisfies the condition.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// No element satisfies the condition.
        /// </exception>
        internal static T First<T>(this in SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount > 0)
            {
                for (var index = 0; index < sourceCount; index++)
                {
                    var value = source[index];

                    if (predicate(value))
                    {
                        return value;
                    }
                }
            }

            throw new InvalidOperationException("nothing found");
        }

        /// <summary>
        /// Finds the first element in the <see cref="SyntaxList{T}"/> that satisfies the specified condition, or the default value if no such element is found.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The first element that satisfies the condition, or the default value if no such element is found.
        /// </returns>
        internal static T FirstOrDefault<T>(this in SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount > 0)
            {
                for (var index = 0; index < sourceCount; index++)
                {
                    var value = source[index];

                    if (predicate(value))
                    {
                        return value;
                    }
                }
            }

            return default;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxList{T}"/> contains no elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the list contains no elements; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this in SyntaxList<T> source) where T : SyntaxNode => source.Count is 0;

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxList{T}"/> contains no elements of the specified kind.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to evaluate.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the kind of syntax node to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the list contains no elements of the specified kind; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this in SyntaxList<T> source, in SyntaxKind kind) where T : SyntaxNode => source.IndexOf(kind) is -1;

        /// <summary>
        /// Determines whether the specified <see cref="SyntaxList{T}"/> contains no elements that satisfy the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if no elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool None<T>(this in SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode => source.All(_ => predicate(_) is false);

        /// <summary>
        /// Finds the last element in the <see cref="SyntaxList{T}"/> that satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The last element that satisfies the condition.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// No element satisfies the condition.
        /// </exception>
        internal static T Last<T>(this in SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            for (var index = source.Count - 1; index >= 0; --index)
            {
                var value = source[index];

                if (predicate(value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException("nothing found");
        }

        /// <summary>
        /// Finds the last element in the <see cref="SyntaxList{T}"/> that satisfies the specified condition, or the default value if no such element is found.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The last element that satisfies the condition, or the default value if no such element is found.
        /// </returns>
        internal static T LastOrDefault<T>(this in SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            for (var index = source.Count - 1; index >= 0; --index)
            {
                var value = source[index];

                if (predicate(value))
                {
                    return value;
                }
            }

            return default;
        }

        /// <summary>
        /// Projects each element of a <see cref="SyntaxList{T}"/> into a new form.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the value returned by the selector.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transformation on each element of the source list.
        /// </returns>
        internal static IEnumerable<TResult> Select<T, TResult>(this SyntaxList<T> source, Func<T, TResult> selector) where T : SyntaxNode
                                                                                                                      where TResult : class
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount > 0)
            {
                for (var index = 0; index < sourceCount; index++)
                {
                    yield return selector(source[index]);
                }
            }
        }

        /// <summary>
        /// Projects each element of a <see cref="SyntaxList{T}"/> into a <see cref="SyntaxToken"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transformation on each element of the source list.
        /// </returns>
        internal static IEnumerable<SyntaxToken> Select<T>(this SyntaxList<T> source, Func<T, SyntaxToken> selector) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount > 0)
            {
                for (var index = 0; index < sourceCount; index++)
                {
                    yield return selector(source[index]);
                }
            }
        }

        /// <summary>
        /// Projects each element of a syntax list to an <see cref="IEnumerable{T}"/> and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the source list.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the elements of the resulting sequence.
        /// </typeparam>
        /// <param name="source">
        /// The syntax list to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each syntax node.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the one-to-many transformation on each element of the source list.
        /// </returns>
        internal static IEnumerable<TResult> SelectMany<T, TResult>(this SyntaxList<T> source, Func<T, IEnumerable<TResult>> selector) where T : SyntaxNode
                                                                                                                                       where TResult : class
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount > 0)
            {
                for (var index = 0; index < sourceCount; index++)
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
        /// Bypasses a specified number of elements in a syntax list and then returns the remaining elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The syntax list to skip elements from.
        /// </param>
        /// <param name="count">
        /// The number of elements to skip.
        /// </param>
        /// <returns>
        /// A sequence that contains the elements of the syntax list after the specified number of elements have been skipped.
        /// </returns>
        internal static IEnumerable<T> Skip<T>(this in SyntaxList<T> source, in int count) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var difference = sourceCount - count;

            if (difference <= 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[difference];

            for (var index = count; index < sourceCount; index++)
            {
                result[index - count] = source[index];
            }

            return result;
        }

        /// <summary>
        /// Converts the specified <see cref="SyntaxList{T}"/> to an array of its elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to convert.
        /// </param>
        /// <returns>
        /// An array of all elements from the list, or an empty array if the list contains no elements.
        /// </returns>
        internal static T[] ToArray<T>(this in SyntaxList<T> source) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount is 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[sourceCount];

            for (var index = 0; index < sourceCount; index++)
            {
                result[index] = source[index];
            }

            return result;
        }

        /// <summary>
        /// Converts the elements of the specified <see cref="SyntaxList{TSource}"/> to a <see cref="HashSet{TResult}"/> by applying the specified selector callback to each element.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of syntax nodes in the source list.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of elements in the resulting hash set.
        /// </typeparam>
        /// <param name="source">
        /// The syntax list whose elements are to be converted.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// A <see cref="HashSet{TResult}"/> that contains the transformed elements from the source list.
        /// </returns>
        internal static HashSet<TResult> ToHashSet<TSource, TResult>(this in SyntaxList<TSource> source, Func<TSource, TResult> selector) where TSource : SyntaxNode
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
        /// Converts the specified <see cref="SyntaxList{T}"/> to a <see cref="List{T}"/> that contains all elements from the list.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to convert.
        /// </param>
        /// <returns>
        /// A <see cref="List{T}"/> that contains all elements from the list.
        /// </returns>
        internal static List<T> ToList<T>(this in SyntaxList<T> source) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var result = new List<T>(sourceCount);

            if (sourceCount > 0)
            {
                for (var index = 0; index < sourceCount; index++)
                {
                    result.Add(source[index]);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a <see cref="SyntaxList{T}"/> containing a single syntax node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax node.
        /// </typeparam>
        /// <param name="source">
        /// The syntax node to include in the list.
        /// </param>
        /// <returns>
        /// A <see cref="SyntaxList{T}"/> containing the specified syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxList<T> ToSyntaxList<T>(this T source) where T : SyntaxNode => SyntaxFactory.SingletonList(source);

        /// <summary>
        /// Creates a <see cref="SyntaxList{T}"/> from the specified sequence of syntax nodes.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax node.
        /// </typeparam>
        /// <param name="source">
        /// The sequence of syntax nodes to include in the list.
        /// </param>
        /// <returns>
        /// A <see cref="SyntaxList{T}"/> containing the specified syntax nodes.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxList<T> ToSyntaxList<T>(this IEnumerable<T> source) where T : SyntaxNode => SyntaxFactory.List(source); // ncrunch: no coverage

        /// <summary>
        /// Filters the specified <see cref="SyntaxList{T}"/> and returns a sequence of syntax nodes that satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The list of syntax nodes to filter.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each syntax node against.
        /// </param>
        /// <returns>
        /// A sequence of syntax nodes that satisfy the condition.
        /// </returns>
        internal static IEnumerable<T> Where<T>(this SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount > 0)
            {
                for (var index = 0; index < sourceCount; index++)
                {
                    var value = source[index];

                    if (predicate(value))
                    {
                        yield return value;
                    }
                }
            }
        }
    }
}