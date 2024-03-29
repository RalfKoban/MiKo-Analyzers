using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
namespace System.Linq
{
    internal static class EnumerableExtensions
    {
        internal static void AddRange<T>(this ISet<T> set, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                set.Add(value);
            }
        }

        internal static bool All(this SyntaxTriviaList value, Func<SyntaxTrivia, bool> filter)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            for (var index = 0; index < valueCount; index++)
            {
                if (filter(value[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool All<T>(this SyntaxList<T> value, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            for (var index = 0; index < valueCount; index++)
            {
                if (predicate(value[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool All<T>(this SeparatedSyntaxList<T> value, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            for (var index = 0; index < valueCount; index++)
            {
                if (predicate(value[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool All(this ReadOnlySpan<char> source, Func<char, bool> callback)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < source.Length; index++)
            {
                if (callback(source[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool All(this SyntaxTokenList value, Func<SyntaxToken, bool> predicate)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            for (var index = 0; index < valueCount; index++)
            {
                if (predicate(value[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool Any(this SyntaxTriviaList value, Func<SyntaxTrivia, bool> filter)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            for (var index = 0; index < valueCount; index++)
            {
                if (filter(value[index]))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool Any<T>(this SyntaxList<T> value, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            for (var index = 0; index < valueCount; index++)
            {
                if (predicate(value[index]))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool Any<T>(this SeparatedSyntaxList<T> value, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            for (var index = 0; index < valueCount; index++)
            {
                if (predicate(value[index]))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool Any(this ReadOnlySpan<char> value, Func<char, bool> filter)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < value.Length; index++)
            {
                if (filter(value[index]))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool Any(this SyntaxTokenList value, Func<SyntaxToken, bool> predicate)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            for (var index = 0; index < valueCount; index++)
            {
                if (predicate(value[index]))
                {
                    return true;
                }
            }

            return false;
        }

        internal static IEnumerable<TSource> Concat<TSource>(this ImmutableArray<TSource> first, ImmutableArray<TSource> second)
        {
            foreach (var item in first)
            {
                yield return item;
            }

            foreach (var item in second)
            {
                yield return item;
            }
        }

        internal static IEnumerable<SyntaxToken> Concat(this SyntaxTokenList first, SyntaxTokenList second)
        {
            foreach (var item in first)
            {
                yield return item;
            }

            foreach (var item in second)
            {
                yield return item;
            }
        }

        internal static bool Contains<TSource, TTarget>(this ImmutableArray<TSource> source, TTarget value, SymbolEqualityComparer comparer) where TSource : class, ISymbol
                                                                                                                                             where TTarget : class, ISymbol
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceLength = source.Length;

            for (var index = 0; index < sourceLength; index++)
            {
                if (comparer.Equals(source[index], value))
                {
                    return true;
                }
            }

            return false;
        }

        internal static int Count<T>(this ImmutableArray<T> source, Func<T, bool> filter) where T : class, ISymbol
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceLength = source.Length;
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

        internal static int Count<T>(this SeparatedSyntaxList<T> source, Func<T, bool> filter) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;
            var count = 0;

            for (var index = 0; index < sourceCount; index++)
            {
                if (filter(source[index]))
                {
                    count++;
                }
            }

            return count;
        }

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

        internal static IEnumerable<T> Except<T>(this SyntaxList<T> source, T value) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

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

        internal static IEnumerable<T> Except<T>(this SeparatedSyntaxList<T> source, T value) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

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

        internal static IEnumerable<T> Except<T>(this SeparatedSyntaxList<T> first, List<T> second) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var firstCount = first.Count;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Exists<T>(this T[] value, Predicate<T> match) => Array.Exists(value, match);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T Find<T>(this T[] value, Predicate<T> match) => Array.Find(value, match);

        internal static SyntaxToken First(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                if (predicate(value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException("nothing found");
        }

        internal static T First<T>(this SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                if (predicate(value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException("nothing found");
        }

        internal static T First<T>(this SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                if (predicate(value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException("nothing found");
        }

        internal static SyntaxToken FirstOrDefault(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                if (predicate(value))
                {
                    return value;
                }
            }

            return default;
        }

        internal static SyntaxToken FirstOrDefault(this SyntaxTokenList.Reversed source, Func<SyntaxToken, bool> predicate)
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

        internal static T FirstOrDefault<T>(this SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                if (predicate(value))
                {
                    return value;
                }
            }

            return default;
        }

        internal static T FirstOrDefault<T>(this SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                if (predicate(value))
                {
                    return value;
                }
            }

            return default;
        }

        internal static int IndexOf<T>(this T[] source, T value) => Array.IndexOf(source, value);

        internal static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
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

        internal static bool None(this SyntaxTriviaList source) => source.Any() is false;

        internal static bool None<T>(this SyntaxList<T> source) where T : SyntaxNode => source.Any() is false;

        internal static bool None<T>(this SeparatedSyntaxList<T> source) where T : SyntaxNode => source.Any() is false;

        internal static bool None(this SyntaxTokenList source) => source.Any() is false;

        internal static bool None<T>(this IEnumerable<T> source) => source.Any() is false;

        internal static bool None<T>(this ImmutableArray<T> source) => source.Any() is false;

        internal static bool None<T>(this SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode => source.All(_ => predicate(_) is false);

        internal static bool None<T>(this SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode => source.All(_ => predicate(_) is false);

        internal static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) => source.All(_ => predicate(_) is false);

        internal static bool MoreThan<T>(this IEnumerable<T> source, int count)
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

        internal static SyntaxToken Last(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
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

        internal static T Last<T>(this SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
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

        internal static T Last<T>(this SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
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

        internal static SyntaxToken LastOrDefault(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
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

        internal static T LastOrDefault<T>(this SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
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

        internal static T LastOrDefault<T>(this SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
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

        internal static IEnumerable<SyntaxToken> Select(this SyntaxTokenList source, Func<SyntaxToken, SyntaxToken> predicate)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                yield return predicate(value);
            }
        }

        internal static IEnumerable<TResult> Select<T, TResult>(this SyntaxList<T> source, Func<T, TResult> selector) where T : SyntaxNode
                                                                                                                      where TResult : class
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                yield return selector(source[index]);
            }
        }

        internal static IEnumerable<TResult> Select<T, TResult>(this SeparatedSyntaxList<T> source, Func<T, TResult> selector) where T : SyntaxNode
                                                                                                                               where TResult : class
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                yield return selector(source[index]);
            }
        }

        internal static IEnumerable<string> Select<T>(this SeparatedSyntaxList<T> source, Func<T, string> selector) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                yield return selector(source[index]);
            }
        }

        internal static IEnumerable<SyntaxToken> Select<T>(this SyntaxList<T> source, Func<T, SyntaxToken> selector) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                yield return selector(source[index]);
            }
        }

        internal static IEnumerable<SyntaxToken> Select<T>(this SeparatedSyntaxList<T> source, Func<T, SyntaxToken> selector) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                yield return selector(source[index]);
            }
        }

        internal static IEnumerable<SyntaxToken> SelectMany<T>(this IEnumerable<T> source, Func<T, SyntaxTokenList> selector) where T : SyntaxNode
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var value in source)
            {
                var list = selector(value);

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var listCount = list.Count;

                for (var index = 0; index < listCount; index++)
                {
                    yield return list[index];
                }
            }
        }

        internal static IEnumerable<TResult> SelectMany<T, TResult>(this IEnumerable<T> source, Func<T, SeparatedSyntaxList<TResult>> selector) where T : SyntaxNode
                                                                                                                                                where TResult : SyntaxNode
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var value in source)
            {
                var list = selector(value);

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var listCount = list.Count;

                for (var index = 0; index < listCount; index++)
                {
                    yield return list[index];
                }
            }
        }

        internal static IEnumerable<TResult> SelectMany<T, TResult>(this ImmutableArray<T> source, Func<T, IEnumerable<TResult>> selector) where T : ISymbol
                                                                                                                                           where TResult : class
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceLength = source.Length;

            for (var index = 0; index < sourceLength; index++)
            {
                var value = source[index];

                foreach (var item in selector(value))
                {
                    yield return item;
                }
            }
        }

        internal static IEnumerable<TResult> SelectMany<T, TResult>(this SyntaxList<T> source, Func<T, IEnumerable<TResult>> selector) where T : SyntaxNode
                                                                                                                                       where TResult : class
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                foreach (var item in selector(value))
                {
                    yield return item;
                }
            }
        }

        internal static IEnumerable<TResult> SelectMany<T, TResult>(this SeparatedSyntaxList<T> source, Func<T, IEnumerable<TResult>> selector) where T : SyntaxNode
                                                                                                                                                where TResult : class
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                foreach (var item in selector(value))
                {
                    yield return item;
                }
            }
        }

        internal static IEnumerable<TResult> SelectMany<T, TResult>(this IEnumerable<T> source, Func<T, ImmutableArray<TResult>> selector) where T : ISymbol
                                                                                                                                           where TResult : ISymbol
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var value in source)
            {
                var array = selector(value);

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var arrayLength = array.Length;

                for (var index = 0; index < arrayLength; index++)
                {
                    yield return array[index];
                }
            }
        }

        internal static IEnumerable<T> Skip<T>(this SyntaxList<T> source, int count) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var difference = sourceCount - count;

            if (difference <= 0)
            {
                return Enumerable.Empty<T>();
            }

            var result = new T[difference];

            for (var index = count; index < sourceCount; index++)
            {
                result[index - count] = source[index];
            }

            return result;
        }

        internal static IEnumerable<T> Skip<T>(this SeparatedSyntaxList<T> source, int count) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var difference = sourceCount - count;

            if (difference <= 0)
            {
                return Enumerable.Empty<T>();
            }

            var result = new T[difference];

            for (var index = count; index < sourceCount; index++)
            {
                result[index - count] = source[index];
            }

            return result;
        }

        internal static IEnumerable<SyntaxToken> Skip(this SyntaxTokenList source, int count)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var difference = sourceCount - count;

            if (difference <= 0)
            {
                return Enumerable.Empty<SyntaxToken>();
            }

            var result = new SyntaxToken[difference];

            for (var index = count; index < sourceCount; index++)
            {
                result[index - count] = source[index];
            }

            return result;
        }

        internal static SyntaxToken[] ToArray(this SyntaxTokenList source)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var target = new SyntaxToken[sourceCount];

            for (var index = 0; index < sourceCount; index++)
            {
                target[index] = source[index];
            }

            return target;
        }

        internal static T[] ToArray<T>(this SyntaxList<T> source) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var target = new T[sourceCount];

            for (var index = 0; index < sourceCount; index++)
            {
                target[index] = source[index];
            }

            return target;
        }

        internal static T[] ToArray<T>(this SeparatedSyntaxList<T> source) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var target = new T[sourceCount];

            for (var index = 0; index < sourceCount; index++)
            {
                target[index] = source[index];
            }

            return target;
        }

        internal static T[] ToArray<T>(this IEnumerable<T> source, IComparer<T> comparer) => source.ToArray(_ => _, comparer);

        internal static TSource[] ToArray<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) => source.OrderBy(keySelector, comparer).ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) => new HashSet<T>(source, comparer);

        internal static HashSet<TResult> ToHashSet<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(selector).ToHashSet();

        internal static HashSet<TResult> ToHashSet<TSource, TResult>(this ImmutableArray<TSource> source, Func<TSource, TResult> selector) => source.Select(selector).ToHashSet();

        internal static List<SyntaxToken> ToList(this SyntaxTokenList source)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var target = new List<SyntaxToken>(sourceCount);

            for (var index = 0; index < sourceCount; index++)
            {
                target.Add(source[index]);
            }

            return target;
        }

        internal static List<T> ToList<T>(this SyntaxList<T> source) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var target = new List<T>(sourceCount);

            for (var index = 0; index < sourceCount; index++)
            {
                target.Add(source[index]);
            }

            return target;
        }

        internal static List<T> ToList<T>(this SeparatedSyntaxList<T> source) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            var target = new List<T>(sourceCount);

            for (var index = 0; index < sourceCount; index++)
            {
                target.Add(source[index]);
            }

            return target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxList<T> ToSyntaxList<T>(this IEnumerable<T> source) where T : SyntaxNode => SyntaxFactory.List(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SeparatedSyntaxList<T> ToSeparatedSyntaxList<T>(this IEnumerable<T> source) where T : SyntaxNode => SyntaxFactory.SeparatedList(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxTokenList ToTokenList(this IEnumerable<SyntaxToken> source) => SyntaxFactory.TokenList(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TrueForAll<T>(this T[] source, Predicate<T> match) => Array.TrueForAll(source, match);

        internal static IEnumerable<SyntaxToken> Where(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                if (predicate(value))
                {
                    yield return value;
                }
            }
        }

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

        internal static IEnumerable<T> Where<T>(this SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            for (var index = 0; index < sourceCount; index++)
            {
                var value = source[index];

                if (predicate(value))
                {
                    yield return value;
                }
            }
        }

        internal static IEnumerable<T> Where<T>(this SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

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