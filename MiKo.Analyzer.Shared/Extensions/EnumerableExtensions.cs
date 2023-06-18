using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    internal static class EnumerableExtensions
    {
        internal static bool All(this SyntaxTriviaList value, Predicate<SyntaxTrivia> filter)
        {
            foreach (var trivia in value)
            {
                if (filter(trivia) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool All<T>(this SyntaxList<T> source, Predicate<T> predicate) where T : SyntaxNode
        {
            foreach (var node in source)
            {
                if (predicate(node) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool All<T>(this SeparatedSyntaxList<T> source, Predicate<T> predicate) where T : SyntaxNode
        {
            foreach (var node in source)
            {
                if (predicate(node) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool All(this ReadOnlySpan<char> source, Func<char, bool> callback)
        {
            foreach (var c in source)
            {
                if (callback(c) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool All(this SyntaxTokenList source, Predicate<SyntaxToken> predicate)
        {
            foreach (var node in source)
            {
                if (predicate(node) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool Any(this SyntaxTriviaList value, Predicate<SyntaxTrivia> filter)
        {
            foreach (var trivia in value)
            {
                if (filter(trivia))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool Any<T>(this SyntaxList<T> source, Predicate<T> predicate) where T : SyntaxNode
        {
            foreach (var node in source)
            {
                if (predicate(node))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool Any<T>(this SeparatedSyntaxList<T> source, Predicate<T> predicate) where T : SyntaxNode
        {
            foreach (var node in source)
            {
                if (predicate(node))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool Any(this SyntaxTokenList source, Predicate<SyntaxToken> predicate)
        {
            foreach (var node in source)
            {
                if (predicate(node))
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
            for (var index = 0; index < source.Length; index++)
            {
                if (comparer.Equals(source[index], value))
                {
                    return true;
                }
            }

            return false;
        }

        internal static IEnumerable<T> Except<T>(this IEnumerable<T> source, T value) where T : class
        {
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
            for (var index = 0; index < source.Count; index++)
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
            for (var index = 0; index < source.Count; index++)
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
            for (var index = 0; index < first.Count; index++)
            {
                var item = first[index];

                if (second.Contains(item))
                {
                    continue;
                }

                yield return item;
            }
        }

        internal static SyntaxToken First(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
        {
            for (var index = 0; index < source.Count; index++)
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
            for (var index = 0; index < source.Count; index++)
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
            for (var index = 0; index < source.Count; index++)
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
            for (var index = 0; index < source.Count; index++)
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
            for (var index = 0; index < source.Count; index++)
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
            for (var index = 0; index < source.Count; index++)
            {
                var value = source[index];

                if (predicate(value))
                {
                    return value;
                }
            }

            return default;
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
                        for (var i = 0; i <= count; i++)
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
            for (var index = 0; index < source.Count; index++)
            {
                var value = source[index];

                yield return predicate(value);
            }
        }

        internal static IEnumerable<TResult> Select<T, TResult>(this SyntaxList<T> source, Func<T, TResult> selector) where T : SyntaxNode
                                                                                                                      where TResult : class
        {
            for (var index = 0; index < source.Count; index++)
            {
                yield return selector(source[index]);
            }
        }

        internal static IEnumerable<TResult> Select<T, TResult>(this SeparatedSyntaxList<T> source, Func<T, TResult> selector) where T : SyntaxNode
                                                                                                                               where TResult : class
        {
            for (var index = 0; index < source.Count; index++)
            {
                yield return selector(source[index]);
            }
        }

        internal static IEnumerable<string> Select<T>(this SeparatedSyntaxList<T> source, Func<T, string> selector) where T : SyntaxNode
        {
            for (var index = 0; index < source.Count; index++)
            {
                yield return selector(source[index]);
            }
        }

        internal static IEnumerable<SyntaxToken> Select<T>(this SyntaxList<T> source, Func<T, SyntaxToken> selector) where T : SyntaxNode
        {
            for (var index = 0; index < source.Count; index++)
            {
                yield return selector(source[index]);
            }
        }

        internal static IEnumerable<SyntaxToken> Select<T>(this SeparatedSyntaxList<T> source, Func<T, SyntaxToken> selector) where T : SyntaxNode
        {
            for (var index = 0; index < source.Count; index++)
            {
                yield return selector(source[index]);
            }
        }

        internal static IEnumerable<SyntaxToken> SelectMany<T>(this IEnumerable<T> source, Func<T, SyntaxTokenList> selector) where T : SyntaxNode
        {
            foreach (var value in source)
            {
                var list = selector(value);

                for (var i = 0; i < list.Count; i++)
                {
                    yield return list[i];
                }
            }
        }

        internal static IEnumerable<TResult> SelectMany<T, TResult>(this IEnumerable<T> source, Func<T, SeparatedSyntaxList<TResult>> selector) where T : SyntaxNode
                                                                                                                                                where TResult : SyntaxNode
        {
            foreach (var value in source)
            {
                var list = selector(value);

                for (var i = 0; i < list.Count; i++)
                {
                    yield return list[i];
                }
            }
        }

        internal static IEnumerable<TResult> SelectMany<T, TResult>(this ImmutableArray<T> source, Func<T, IEnumerable<TResult>> selector) where T : ISymbol
                                                                                                                                           where TResult : class
        {
            foreach (var value in source)
            {
                foreach (var item in selector(value))
                {
                    yield return item;
                }
            }
        }

        internal static IEnumerable<TResult> SelectMany<T, TResult>(this SyntaxList<T> source, Func<T, IEnumerable<TResult>> selector) where T : SyntaxNode
                                                                                                                                       where TResult : class
        {
            foreach (var value in source)
            {
                foreach (var item in selector(value))
                {
                    yield return item;
                }
            }
        }

        internal static IEnumerable<TResult> SelectMany<T, TResult>(this SeparatedSyntaxList<T> source, Func<T, IEnumerable<TResult>> selector) where T : SyntaxNode
                                                                                                                                                where TResult : class
        {
            foreach (var value in source)
            {
                foreach (var item in selector(value))
                {
                    yield return item;
                }
            }
        }

        internal static IEnumerable<TResult> SelectMany<T, TResult>(this IEnumerable<T> source, Func<T, ImmutableArray<TResult>> selector) where T : ISymbol
                                                                                                                                           where TResult : ISymbol
        {
            foreach (var value in source)
            {
                var array = selector(value);

                for (var i = 0; i < array.Length; i++)
                {
                    yield return array[i];
                }
            }
        }

        internal static IEnumerable<T> Skip<T>(this SyntaxList<T> source, int count) where T : SyntaxNode
        {
            for (var index = count; index < source.Count; index++)
            {
                yield return source[index];
            }
        }

        internal static IEnumerable<T> Skip<T>(this SeparatedSyntaxList<T> source, int count) where T : SyntaxNode
        {
            for (var index = count; index < source.Count; index++)
            {
                yield return source[index];
            }
        }

        internal static IEnumerable<SyntaxToken> Skip(this SyntaxTokenList source, int count)
        {
            for (var index = count; index < source.Count; index++)
            {
                yield return source[index];
            }
        }

        internal static SyntaxToken[] ToArray(this SyntaxTokenList source)
        {
            var target = new SyntaxToken[source.Count];

            for (var index = 0; index < source.Count; index++)
            {
                target[index] = source[index];
            }

            return target;
        }

        internal static T[] ToArray<T>(this SyntaxList<T> source) where T : SyntaxNode
        {
            var target = new T[source.Count];

            for (var index = 0; index < source.Count; index++)
            {
                target[index] = source[index];
            }

            return target;
        }

        internal static T[] ToArray<T>(this SeparatedSyntaxList<T> source) where T : SyntaxNode
        {
            var target = new T[source.Count];

            for (var index = 0; index < source.Count; index++)
            {
                target[index] = source[index];
            }

            return target;
        }

        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);

        internal static HashSet<TResult> ToHashSet<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(selector).ToHashSet();

        internal static HashSet<TResult> ToHashSet<TSource, TResult>(this ImmutableArray<TSource> source, Func<TSource, TResult> selector) => source.Select(selector).ToHashSet();

        internal static List<SyntaxToken> ToList(this SyntaxTokenList source)
        {
            var target = new List<SyntaxToken>(source.Count);

            for (var index = 0; index < source.Count; index++)
            {
                target.Add(source[index]);
            }

            return target;
        }

        internal static List<T> ToList<T>(this SyntaxList<T> source) where T : SyntaxNode
        {
            var target = new List<T>(source.Count);

            for (var index = 0; index < source.Count; index++)
            {
                target.Add(source[index]);
            }

            return target;
        }

        internal static List<T> ToList<T>(this SeparatedSyntaxList<T> source) where T : SyntaxNode
        {
            var target = new List<T>(source.Count);

            for (var index = 0; index < source.Count; index++)
            {
                target.Add(source[index]);
            }

            return target;
        }

        internal static IEnumerable<SyntaxToken> Where(this SyntaxTokenList source, Func<SyntaxToken, bool> predicate)
        {
            for (var index = 0; index < source.Count; index++)
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
            for (var index = 0; index < source.Count; index++)
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
            for (var index = 0; index < source.Count; index++)
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