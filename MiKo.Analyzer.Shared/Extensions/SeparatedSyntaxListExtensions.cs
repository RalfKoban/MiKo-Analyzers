using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    internal static class SeparatedSyntaxListExtensions
    {
        /// <summary>
        /// Determines whether all elements in the <see cref="SeparatedSyntaxList{T}"/> satisfy the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="value">
        /// The separated list of syntax nodes to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool All<T>(this in SeparatedSyntaxList<T> value, Func<T, bool> predicate) where T : SyntaxNode
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
        /// Determines whether any element in the <see cref="SeparatedSyntaxList{T}"/> satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="value">
        /// The separated list of syntax nodes to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any element satisfies the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Any<T>(this in SeparatedSyntaxList<T> value, Func<T, bool> predicate) where T : SyntaxNode
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
        /// Adds all elements from the specified collection to the <see cref="HashSet{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the set.
        /// </typeparam>
        /// <param name="set">
        /// The set to which the elements are added.
        /// </param>
        /// <param name="values">
        /// The collection of elements to add.
        /// </param>
        internal static void AddRange<T>(this HashSet<T> set, IEnumerable<T> values)
        {
            switch (values)
            {
                case IReadOnlyCollection<T> rc when rc.Count is 0:
                case ICollection<T> c when c.Count is 0:
                    return;
            }

            set.UnionWith(values);
        }

        /// <summary>
        /// Counts the number of elements in the specified <see cref="SeparatedSyntaxList{T}"/> that satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="value">
        /// The separated list to evaluate.
        /// </param>
        /// <param name="filter">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The number of elements that satisfy the condition.
        /// </returns>
        internal static int Count<T>(this in SeparatedSyntaxList<T> value, Func<T, bool> filter) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            if (valueCount <= 0)
            {
                return 0;
            }

            var count = 0;

            for (var index = 0; index < valueCount; index++)
            {
                if (filter(value[index]))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Provides a sequence that excludes the specified value from the <see cref="SeparatedSyntaxList{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The source separated list.
        /// </param>
        /// <param name="value">
        /// The value to exclude from the list.
        /// </param>
        /// <returns>
        /// A sequence that contains all elements from the source sequence except the specified value.
        /// </returns>
        internal static IEnumerable<T> Except<T>(this SeparatedSyntaxList<T> source, T value) where T : SyntaxNode
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
        /// Provides a sequence that excludes the elements in the specified collection from the <see cref="SeparatedSyntaxList{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="first">
        /// The source separated list.
        /// </param>
        /// <param name="second">
        /// The collection of elements to exclude from the list.
        /// </param>
        /// <returns>
        /// A sequence that contains all elements from the source sequence except the specified elements.
        /// </returns>
        internal static IEnumerable<T> Except<T>(this SeparatedSyntaxList<T> first, IReadOnlyCollection<T> second) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var firstCount = first.Count;

            if (firstCount > 0)
            {
                for (var index = 0; index < firstCount; index++)
                {
                    var item = first[index];

                    if (second.Contains(item))
                    {
                        continue;
                    }

                    yield return item;
                }
            }
        }

        /// <summary>
        /// Finds the first element in the <see cref="SeparatedSyntaxList{T}"/> that satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to search.
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
        internal static T First<T>(this in SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
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
        /// Finds the first element in the <see cref="SeparatedSyntaxList{T}"/> that satisfies the specified condition, or the default value if no such element is found.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The first element that satisfies the condition, or the default value if no such element is found.
        /// </returns>
        internal static T FirstOrDefault<T>(this in SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
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

        internal static IEnumerable<string> GetNames(this in SeparatedSyntaxList<VariableDeclaratorSyntax> value) => value.Select(_ => _.GetName());

        /// <summary>
        /// Determines whether the specified <see cref="SeparatedSyntaxList{T}"/> contains no elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the list contains no elements; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this in SeparatedSyntaxList<T> source) where T : SyntaxNode => source.Count is 0;

        /// <summary>
        /// Determines whether the specified <see cref="SeparatedSyntaxList{T}"/> contains no elements of the specified kind.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to evaluate.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the kind of syntax node to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the list contains no elements of the specified kind; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this in SeparatedSyntaxList<T> source, in SyntaxKind kind) where T : SyntaxNode => source.IndexOf(kind) is -1;

        /// <summary>
        /// Determines whether the specified <see cref="SeparatedSyntaxList{T}"/> contains no elements that satisfy the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if no elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool None<T>(this in SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode => source.All(_ => predicate(_) is false);

        /// <summary>
        /// Finds the last element in the <see cref="SeparatedSyntaxList{T}"/> that satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to search.
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
        internal static T Last<T>(this in SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
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
        /// Finds the last element in the <see cref="SeparatedSyntaxList{T}"/> that satisfies the specified condition, or the default value if no such element is found.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The last element that satisfies the condition, or the default value if no such element is found.
        /// </returns>
        internal static T LastOrDefault<T>(this in SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
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

        internal static IReadOnlyList<TResult> OfKind<TResult, TSyntaxNode>(this in SeparatedSyntaxList<TSyntaxNode> source, in SyntaxKind kind) where TSyntaxNode : SyntaxNode
                                                                                                                                                 where TResult : TSyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount is 0)
            {
                return Array.Empty<TResult>();
            }

            var results = new List<TResult>();

            for (var index = 0; index < sourceCount; index++)
            {
                var item = source[index];

                if (item.IsKind(kind))
                {
                    results.Add((TResult)item);
                }
            }

            return results;
        }

        /// <summary>
        /// Projects each element of a <see cref="SeparatedSyntaxList{T}"/> into a new form.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the value returned by the selector.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transformation on each element of the source list.
        /// </returns>
        internal static IEnumerable<TResult> Select<T, TResult>(this SeparatedSyntaxList<T> source, Func<T, TResult> selector) where T : SyntaxNode
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
        /// Projects each element of a <see cref="SeparatedSyntaxList{T}"/> into a string.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transformation on each element of the source list.
        /// </returns>
        internal static IEnumerable<string> Select<T>(this SeparatedSyntaxList<T> source, Func<T, string> selector) where T : SyntaxNode
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
        /// Projects each element of a <see cref="SeparatedSyntaxList{T}"/> into a <see cref="SyntaxToken"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transformation on each element of the source list.
        /// </returns>
        internal static IEnumerable<SyntaxToken> Select<T>(this SeparatedSyntaxList<T> source, Func<T, SyntaxToken> selector) where T : SyntaxNode
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
        /// Projects each element of a sequence of syntax nodes into a sequence of syntax nodes of another type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the source sequence.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of syntax nodes in the resulting sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence of syntax nodes to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each syntax node.
        /// </param>
        /// <returns>
        /// A sequence of syntax nodes resulting from applying the transformation to each syntax node in the source sequence.
        /// </returns>
        internal static IEnumerable<TResult> SelectMany<T, TResult>(this IEnumerable<T> source, Func<T, SeparatedSyntaxList<TResult>> selector) where T : SyntaxNode
                                                                                                                                                where TResult : SyntaxNode
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery, so there is no need for a Select clause
            foreach (var value in source)
            {
                var list = selector(value);

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var listCount = list.Count;

                if (listCount > 0)
                {
                    for (var index = 0; index < listCount; index++)
                    {
                        yield return list[index];
                    }
                }
            }
        }

        /// <summary>
        /// Projects each element of a separated syntax list to an <see cref="IEnumerable{T}"/> and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the source list.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the elements of the resulting sequence.
        /// </typeparam>
        /// <param name="source">
        /// The separated syntax list to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each syntax node.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the one-to-many transformation on each element of the source list.
        /// </returns>
        internal static IEnumerable<TResult> SelectMany<T, TResult>(this SeparatedSyntaxList<T> source, Func<T, IEnumerable<TResult>> selector) where T : SyntaxNode
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
        /// Bypasses a specified number of elements in a separated syntax list and then returns the remaining elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the separated list.
        /// </typeparam>
        /// <param name="source">
        /// The separated syntax list to skip elements from.
        /// </param>
        /// <param name="count">
        /// The number of elements to skip.
        /// </param>
        /// <returns>
        /// An array of <typeparamref name="T"/> that contains the elements of the separated syntax list after the specified number of elements have been skipped.
        /// </returns>
        internal static T[] Skip<T>(this in SeparatedSyntaxList<T> source, in int count) where T : SyntaxNode
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
        /// Converts the specified <see cref="SeparatedSyntaxList{T}"/> to an array of its elements.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the separated list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to convert.
        /// </param>
        /// <returns>
        /// An array of all elements from the separated list, or an empty array if the list contains no elements.
        /// </returns>
        internal static T[] ToArray<T>(this in SeparatedSyntaxList<T> source) where T : SyntaxNode
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
        /// Converts the specified <see cref="SeparatedSyntaxList{TSource}"/> to an array of keys by applying the specified selector callback.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key returned by the selector.
        /// </typeparam>
        /// <typeparam name="TSource">
        /// The type of syntax nodes in the separated list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to convert.
        /// </param>
        /// <param name="keySelector">
        /// A callback to extract a key from each element.
        /// </param>
        /// <returns>
        /// An array of keys extracted from each element in the separated list, or an empty array if the list contains no elements.
        /// </returns>
        internal static TKey[] ToArray<TKey, TSource>(this in SeparatedSyntaxList<TSource> source, Func<TSource, TKey> keySelector) where TSource : SyntaxNode
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

//// ncrunch: no coverage start

        /// <summary>
        /// Converts the elements of the specified <see cref="SeparatedSyntaxList{TSource}"/> to a <see cref="HashSet{TResult}"/> by applying the specified selector callback to each element.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of syntax nodes in the source separated list.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of elements in the resulting hash set.
        /// </typeparam>
        /// <param name="source">
        /// The separated syntax list whose elements are to be converted.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// A <see cref="HashSet{TResult}"/> that contains the transformed elements from the source separated list.
        /// </returns>
        internal static HashSet<TResult> ToHashSet<TSource, TResult>(this in SeparatedSyntaxList<TSource> source, Func<TSource, TResult> selector) where TSource : SyntaxNode
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
        /// Converts the specified <see cref="SeparatedSyntaxList{T}"/> to a <see cref="List{T}"/> that contains all elements from the separated list.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the separated list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to convert.
        /// </param>
        /// <returns>
        /// A <see cref="List{T}"/> that contains all elements from the separated list.
        /// </returns>
        internal static List<T> ToList<T>(this in SeparatedSyntaxList<T> source) where T : SyntaxNode
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

//// ncrunch: no coverage end

        /// <summary>
        /// Creates a <see cref="SeparatedSyntaxList{T}"/> containing a single syntax node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax node.
        /// </typeparam>
        /// <param name="source">
        /// The syntax node to include in the separated list.
        /// </param>
        /// <returns>
        /// A <see cref="SeparatedSyntaxList{T}"/> containing the specified syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SeparatedSyntaxList<T> ToSeparatedSyntaxList<T>(this T source) where T : SyntaxNode => SyntaxFactory.SingletonSeparatedList(source);

        /// <summary>
        /// Creates a <see cref="SeparatedSyntaxList{T}"/> from the specified sequence of syntax nodes.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax node.
        /// </typeparam>
        /// <param name="source">
        /// The sequence of syntax nodes to include in the separated list.
        /// </param>
        /// <returns>
        /// A <see cref="SeparatedSyntaxList{T}"/> containing the specified syntax nodes.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SeparatedSyntaxList<T> ToSeparatedSyntaxList<T>(this IEnumerable<T> source) where T : SyntaxNode => SyntaxFactory.SeparatedList(source);

        /// <summary>
        /// Filters the specified <see cref="SeparatedSyntaxList{T}"/> and returns a sequence of syntax nodes that satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the separated list.
        /// </typeparam>
        /// <param name="source">
        /// The separated list of syntax nodes to filter.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each syntax node against.
        /// </param>
        /// <returns>
        /// A sequence of syntax nodes that satisfy the condition.
        /// </returns>
        internal static IEnumerable<T> Where<T>(this SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
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