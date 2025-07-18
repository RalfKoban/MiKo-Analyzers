﻿using System.Collections.Generic;
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

        internal static HashSet<T> Except<T>(this HashSet<T> source, IEnumerable<T> values) where T : class
        {
            source.ExceptWith(values);

            return source;
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

        internal static HashSet<T> Except<T>(this HashSet<T> source, params T[] values) where T : class
        {
            source.ExceptWith(values);

            return source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Exists<T>(this T[] value, Predicate<T> match) => value.Length > 0 && Array.Exists(value, match);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T Find<T>(this T[] value, Predicate<T> match) => value.Length > 0 ? Array.Find(value, match) : default;

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

        internal static int IndexOf<T>(this T[] source, T value) => source.Length is 0 ? -1 : Array.IndexOf(source, value);

        internal static int IndexOf<T>(this T[] source, Func<T, bool> predicate) => source.Length is 0 ? -1 : source.IndexOf(new Predicate<T>(predicate));

        internal static int IndexOf<T>(this T[] source, Predicate<T> predicate) => source.Length is 0 ? -1 : Array.FindIndex(source, predicate);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsEmptyArray<T>(this IEnumerable<T> source) => source is T[] array && array.Length is 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None(this in SyntaxTriviaList source) => source.Count is 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this in SyntaxList<T> source) where T : SyntaxNode => source.Count is 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this in SeparatedSyntaxList<T> source) where T : SyntaxNode => source.Count is 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None(this in SyntaxTokenList source) => source.Count is 0;

        internal static bool None<T>(this IEnumerable<T> source) => source.Any() is false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this IReadOnlyCollection<T> source) => source.Count is 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this in ImmutableArray<T> source) => source.Length is 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this in SyntaxList<T> source, in SyntaxKind kind) where T : SyntaxNode => source.IndexOf(kind) is -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None(this in SyntaxTokenList source, in SyntaxKind kind) => source.Any(kind) is false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool None<T>(this in SeparatedSyntaxList<T> source, in SyntaxKind kind) where T : SyntaxNode => source.IndexOf(kind) is -1;

        internal static bool None<T>(this in SyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode => source.All(_ => predicate(_) is false);

        internal static bool None<T>(this in SeparatedSyntaxList<T> source, Func<T, bool> predicate) where T : SyntaxNode => source.All(_ => predicate(_) is false);

        internal static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) => source.All(_ => predicate(_) is false);

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

        internal static T[] ToArray<T>(this IEnumerable<T> source, IComparer<T> comparer) => source.ToArray(_ => _, comparer);

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

        internal static TSource[] ToArray<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) => source.OrderBy(keySelector, comparer).ToArray(); // ncrunch: no coverage

#if NETSTANDARD2_0

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source); // ncrunch: no coverage

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) => new HashSet<T>(source, comparer);

#endif

//// ncrunch: no coverage start

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxList<T> ToSyntaxList<T>(this T source) where T : SyntaxNode => SyntaxFactory.SingletonList(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxList<T> ToSyntaxList<T>(this IEnumerable<T> source) where T : SyntaxNode => SyntaxFactory.List(source); // ncrunch: no coverage

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SeparatedSyntaxList<T> ToSeparatedSyntaxList<T>(this T value) where T : SyntaxNode => SyntaxFactory.SingletonSeparatedList(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SeparatedSyntaxList<T> ToSeparatedSyntaxList<T>(this IEnumerable<T> source) where T : SyntaxNode => SyntaxFactory.SeparatedList(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SyntaxTokenList ToTokenList(this IEnumerable<SyntaxToken> source) => SyntaxFactory.TokenList(source); // ncrunch: no coverage

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TrueForAll<T>(this T[] source, Predicate<T> match) => Array.TrueForAll(source, match);

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