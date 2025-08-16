using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System.Linq
{
    internal static class EnumerableExtensions
    {
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
        /// Determines whether all elements in the <see cref="SyntaxTriviaList"/> satisfy the specified condition.
        /// </summary>
        /// <param name="value">
        /// The list of syntax trivia to evaluate.
        /// </param>
        /// <param name="filter">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool All(this in SyntaxTriviaList value, Func<SyntaxTrivia, bool> filter)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            if (valueCount > 0)
            {
                for (var index = 0; index < valueCount; index++)
                {
                    if (filter(value[index]) is false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

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
        /// Determines whether all elements in the <see cref="SyntaxTokenList"/> satisfy the specified condition.
        /// </summary>
        /// <param name="value">
        /// The list of syntax tokens to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all elements satisfy the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool All(this in SyntaxTokenList value, Func<SyntaxToken, bool> predicate)
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
        /// Determines whether any element in the <see cref="SyntaxTriviaList"/> satisfies the specified condition.
        /// </summary>
        /// <param name="value">
        /// The list of syntax trivia to evaluate.
        /// </param>
        /// <param name="filter">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any element satisfies the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Any(this in SyntaxTriviaList value, Func<SyntaxTrivia, bool> filter)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            if (valueCount > 0)
            {
                for (var index = 0; index < valueCount; index++)
                {
                    if (filter(value[index]))
                    {
                        return true;
                    }
                }
            }

            return false;
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
        /// Determines whether any element in the <see cref="SyntaxTokenList"/> satisfies the specified condition.
        /// </summary>
        /// <param name="value">
        /// The list of syntax tokens to evaluate.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any element satisfies the condition; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Any(this in SyntaxTokenList value, Func<SyntaxToken, bool> predicate)
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
        /// Concatenates two <see cref="SyntaxTokenList"/> sequences into a single sequence.
        /// </summary>
        /// <param name="first">
        /// The first sequence to concatenate.
        /// </param>
        /// <param name="second">
        /// The sequence to concatenate to the first sequence.
        /// </param>
        /// <returns>
        /// A sequence that contains the concatenated elements of the two input sequences.
        /// </returns>
        internal static IEnumerable<SyntaxToken> Concat(this SyntaxTokenList first, SyntaxTokenList second)
        {
            if (first.Count > 0)
            {
                foreach (var item in first)
                {
                    yield return item;
                }
            }

            if (second.Count > 0)
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
        /// Counts the number of elements in the specified <see cref="SyntaxTriviaList"/> that satisfy the given condition.
        /// </summary>
        /// <param name="value">
        /// The list of syntax trivia to evaluate.
        /// </param>
        /// <param name="filter">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The number of elements that satisfy the condition.
        /// </returns>
        internal static int Count(this in SyntaxTriviaList value, Func<SyntaxTrivia, bool> filter)
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
        /// Finds the first element in the <see cref="SyntaxTokenList"/> that satisfies the specified condition.
        /// </summary>
        /// <param name="source">
        /// The list of syntax tokens to search.
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
        internal static SyntaxToken First(this in SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
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
        /// Finds the first element in the <see cref="SyntaxTokenList"/> that satisfies the specified condition, or the default value if no such element is found.
        /// </summary>
        /// <param name="source">
        /// The list of syntax tokens to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The first element that satisfies the condition, or the default value if no such element is found.
        /// </returns>
        internal static SyntaxToken FirstOrDefault(this in SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
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
        /// Finds the first element in the reversed <see cref="SyntaxTokenList"/> that satisfies the specified condition, or the default value if no such element is found.
        /// </summary>
        /// <param name="source">
        /// The reversed list of syntax tokens to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The first element that satisfies the condition, or the default value if no such element is found.
        /// </returns>
        internal static SyntaxToken FirstOrDefault(this in SyntaxTokenList.Reversed source, Func<SyntaxToken, bool> predicate)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var value in source)
            {
                if (predicate(value))
                {
                    return value;
                }
            }

            return default;
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
        /// Determines whether the specified <see cref="SyntaxTriviaList"/> contains no elements.
        /// </summary>
        /// <param name="source">
        /// The list of syntax trivia to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the list contains no elements; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None(this in SyntaxTriviaList source) => source.Count is 0;

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
        /// Determines whether the specified <see cref="SyntaxTokenList"/> contains no elements.
        /// </summary>
        /// <param name="source">
        /// The list of syntax tokens to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the list contains no elements; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None(this in SyntaxTokenList source) => source.Count is 0;

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
        /// Determines whether the specified <see cref="SyntaxTokenList"/> contains no elements of the specified kind.
        /// </summary>
        /// <param name="source">
        /// The list of syntax tokens to evaluate.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the kind of syntax token to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the list contains no elements of the specified kind; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None(this in SyntaxTokenList source, in SyntaxKind kind) => source.Any(kind) is false;

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
        /// Finds the last element in the <see cref="SyntaxTokenList"/> that satisfies the specified condition.
        /// </summary>
        /// <param name="source">
        /// The list of syntax tokens to search.
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
        internal static SyntaxToken Last(this in SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
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
        /// Finds the last element in the <see cref="SyntaxTokenList"/> that satisfies the specified condition, or the default value if no such element is found.
        /// </summary>
        /// <param name="source">
        /// The list of syntax tokens to search.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each element against.
        /// </param>
        /// <returns>
        /// The last element that satisfies the condition, or the default value if no such element is found.
        /// </returns>
        internal static SyntaxToken LastOrDefault(this in SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
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

        /// <summary>
        /// Projects each element of a <see cref="SyntaxTokenList"/> into a new form.
        /// </summary>
        /// <param name="source">
        /// The list of syntax tokens to project.
        /// </param>
        /// <param name="predicate">
        /// A transformation to apply to each element.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transformation on each element of the source list.
        /// </returns>
        internal static IEnumerable<SyntaxToken> Select(this SyntaxTokenList source, Func<SyntaxToken, SyntaxToken> predicate)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount > 0)
            {
                for (var index = 0; index < sourceCount; index++)
                {
                    var value = source[index];

                    yield return predicate(value);
                }
            }
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
        /// Projects each element of a sequence of syntax nodes into a sequence of syntax tokens.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the source sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence of syntax nodes to project.
        /// </param>
        /// <param name="selector">
        /// A transformation to apply to each syntax node.
        /// </param>
        /// <returns>
        /// A sequence of syntax tokens resulting from applying the transformation to each syntax node in the source sequence.
        /// </returns>
        internal static IEnumerable<SyntaxToken> SelectMany<T>(this IEnumerable<T> source, Func<T, SyntaxTokenList> selector) where T : SyntaxNode
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
        /// Bypasses a specified number of elements in a syntax token list and then returns the remaining elements.
        /// </summary>
        /// <param name="source">
        /// The syntax token list to skip elements from.
        /// </param>
        /// <param name="count">
        /// The number of elements to skip.
        /// </param>
        /// <returns>
        /// An array of <see cref="SyntaxToken"/> that contains the elements of the syntax token list after the specified number of elements have been skipped.
        /// </returns>
        internal static SyntaxToken[] Skip(this in SyntaxTokenList source, in int count)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var difference = sourceCount - count;

            if (difference <= 0)
            {
                return Array.Empty<SyntaxToken>();
            }

            var result = new SyntaxToken[difference];

            for (var index = count; index < sourceCount; index++)
            {
                result[index - count] = source[index];
            }

            return result;
        }

        /// <summary>
        /// Converts the specified <see cref="SyntaxTokenList"/> to an array of <see cref="SyntaxToken"/> instances.
        /// </summary>
        /// <param name="source">
        /// The list of syntax tokens to convert.
        /// </param>
        /// <returns>
        /// An array of all syntax tokens from the list, or an empty array if the list contains no elements.
        /// </returns>
        internal static SyntaxToken[] ToArray(this in SyntaxTokenList source)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount is 0)
            {
                return Array.Empty<SyntaxToken>();
            }

            var result = new SyntaxToken[sourceCount];

            for (var index = 0; index < sourceCount; index++)
            {
                result[index] = source[index];
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
                case ImmutableArray<TSource> array: return ToArray(array, keySelector);
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
                case ImmutableArray<TSource> array: return array.ToHashSet(selector);
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

        /// <summary>
        /// Converts the specified <see cref="SyntaxTokenList"/> to a <see cref="List{SyntaxToken}"/>.
        /// </summary>
        /// <param name="source">
        /// The syntax token list to convert.
        /// </param>
        /// <returns>
        /// A <see cref="List{SyntaxToken}"/> that contains all syntax tokens from the list.
        /// </returns>
        internal static List<SyntaxToken> ToList(this in SyntaxTokenList source)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var result = new List<SyntaxToken>(sourceCount);

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
        /// Creates a <see cref="SeparatedSyntaxList{T}"/> containing a single syntax node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to include in the separated list.
        /// </param>
        /// <returns>
        /// A <see cref="SeparatedSyntaxList{T}"/> containing the specified syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SeparatedSyntaxList<T> ToSeparatedSyntaxList<T>(this T value) where T : SyntaxNode => SyntaxFactory.SingletonSeparatedList(value);

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
        /// Creates a <see cref="SyntaxTokenList"/> from the specified sequence of syntax tokens.
        /// </summary>
        /// <param name="source">
        /// The sequence of syntax tokens to include in the list.
        /// </param>
        /// <returns>
        /// A <see cref="SyntaxTokenList"/> containing the specified syntax tokens.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxTokenList ToTokenList(this IEnumerable<SyntaxToken> source) => SyntaxFactory.TokenList(source); // ncrunch: no coverage

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

        /// <summary>
        /// Filters the specified <see cref="SyntaxTokenList"/> and returns a sequence of syntax tokens that satisfy the given condition.
        /// </summary>
        /// <param name="source">
        /// The list of syntax tokens to filter.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each syntax token against.
        /// </param>
        /// <returns>
        /// A sequence of syntax tokens that satisfy the condition.
        /// </returns>
        internal static IEnumerable<SyntaxToken> Where(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
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

        /// <summary>
        /// Filters the specified <see cref="SyntaxTriviaList"/> and returns a sequence of syntax trivia that satisfy the given condition.
        /// </summary>
        /// <param name="source">
        /// The list of syntax trivia to filter.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each syntax trivia against.
        /// </param>
        /// <returns>
        /// A sequence of syntax trivia that satisfy the condition.
        /// </returns>
        internal static IEnumerable<SyntaxTrivia> Where(this SyntaxTriviaList source, Func<SyntaxTrivia, bool> predicate)
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

        /// <summary>
        /// Filters the specified <see cref="SyntaxTokenList.Reversed"/> and returns a sequence of syntax tokens that satisfy the given condition.
        /// </summary>
        /// <param name="source">
        /// The reversed list of syntax tokens to filter.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each syntax token against.
        /// </param>
        /// <returns>
        /// A sequence of syntax tokens that satisfy the condition.
        /// </returns>
        internal static IEnumerable<SyntaxToken> Where(this SyntaxTokenList.Reversed source, Func<SyntaxToken, bool> predicate)
        {
            foreach (var value in source)
            {
                if (predicate(value))
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Filters the specified <see cref="SyntaxTriviaList.Reversed"/> and returns a sequence of syntax trivia that satisfy the given condition.
        /// </summary>
        /// <param name="source">
        /// The reversed list of syntax trivia to filter.
        /// </param>
        /// <param name="predicate">
        /// The condition to test each syntax trivia against.
        /// </param>
        /// <returns>
        /// A sequence of syntax trivia that satisfy the condition.
        /// </returns>
        internal static IEnumerable<SyntaxTrivia> Where(this SyntaxTriviaList.Reversed source, Func<SyntaxTrivia, bool> predicate)
        {
            foreach (var value in source)
            {
                if (predicate(value))
                {
                    yield return value;
                }
            }
        }

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