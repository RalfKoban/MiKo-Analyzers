using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxList{T}"/>s.
    /// </summary>
    internal static class SyntaxListExtensions
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
            var count = value.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
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
            var count = value.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
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
            var count = source.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
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
            var count = source.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
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
            var count = source.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
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
        /// Filters the elements of the specified <see cref="SyntaxList{T}"/> based on a specified type.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type to filter the elements of the sequence on.
        /// </typeparam>
        /// <param name="source">
        /// The list whose elements are to be filtered.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with the elements from the source list that are of the specified type.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IReadOnlyList<TResult> OfType<TResult>(this in SyntaxList<XmlNodeSyntax> source) where TResult : XmlNodeSyntax => source.OfType<XmlNodeSyntax, TResult>();

        /// <summary>
        /// Filters the elements of the specified <see cref="SyntaxList{T}"/> based on a specified type.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type to filter the elements of the sequence on.
        /// </typeparam>
        /// <param name="source">
        /// The list whose elements are to be filtered.
        /// </param>
        /// <returns>
        /// A collection of xml attribute syntaxes with the elements from the source list that are of the specified type.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IReadOnlyList<TResult> OfType<TResult>(this in SyntaxList<XmlAttributeSyntax> source) where TResult : XmlAttributeSyntax => source.OfType<XmlAttributeSyntax, TResult>();

        /// <summary>
        /// Filters the elements of the specified <see cref="SyntaxList{T}"/> based on a specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the source list.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type to filter the elements of the sequence on.
        /// </typeparam>
        /// <param name="source">
        /// The list whose elements are to be filtered.
        /// </param>
        /// <returns>
        /// A collection of the elements from the source list that are of the specified type.
        /// </returns>
        internal static IReadOnlyList<TResult> OfType<T, TResult>(this in SyntaxList<T> source) where T : SyntaxNode
                                                                                                where TResult : T
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var count = source.Count;

            if (count is 0)
            {
                return Array.Empty<TResult>();
            }

            List<TResult> results = null;

            for (var index = 0; index < count; index++)
            {
                if (source[index] is TResult result)
                {
                    if (results is null)
                    {
                        results = new List<TResult>(1);
                    }

                    results.Add(result);
                }
            }

            return results ?? (IReadOnlyList<TResult>)Array.Empty<TResult>();
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with all occurrences of the specified phrase replaced with the specified replacement.
        /// </summary>
        /// <param name="source">
        /// The list in which to replace text.
        /// </param>
        /// <param name="phrase">
        /// The phrase to be replaced.
        /// </param>
        /// <param name="replacement">
        /// The text that replaces all occurrences of the phrase.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with all occurrences of the phrase replaced with the replacement.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> ReplaceText(this in SyntaxList<XmlNodeSyntax> source, string phrase, string replacement) => source.ReplaceText(new[] { phrase }, replacement);

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with all occurrences of the specified phrases replaced with the specified replacement.
        /// </summary>
        /// <param name="source">
        /// The list in which to replace text.
        /// </param>
        /// <param name="phrases">
        /// The phrases to be replaced.
        /// </param>
        /// <param name="replacement">
        /// The text that replaces all occurrences of the phrases.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with all occurrences of the phrases replaced with the replacement.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> ReplaceText(this in SyntaxList<XmlNodeSyntax> source, in ReadOnlySpan<string> phrases, string replacement)
        {
            var count = source.Count;

            if (count is 0)
            {
                return source;
            }

            var result = source.ToArray();

            for (var index = 0; index < count; index++)
            {
                var value = result[index];

                if (value is XmlTextSyntax text)
                {
                    result[index] = text.ReplaceText(phrases, replacement);
                }
            }

            return result.ToSyntaxList();
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
            var count = source.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
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
            var count = source.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
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
            var count = source.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
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
            var count = source.Count;

            if (count is 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[count];

            for (var index = 0; index < count; index++)
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
            var count = source.Count;

            var result = new List<T>(count);

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
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
        /// A collection of syntax nodes with the specified syntax node.
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
        /// A collection of syntax nodes with the specified syntax nodes.
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
            var count = source.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
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
        /// Gets a <see cref="SyntaxList{T}"/> with the first element adjusted to have proper indentation.
        /// </summary>
        /// <typeparam name="T">
        /// The type of syntax nodes in the list.
        /// </typeparam>
        /// <param name="values">
        /// The list whose first element is to be adjusted.
        /// </param>
        /// <returns>
        /// A collection of syntax nodes whose first element is adjusted to have proper indentation.
        /// </returns>
        internal static SyntaxList<T> WithIndentation<T>(this in SyntaxList<T> values) where T : SyntaxNode
        {
            var value = values[0];

            return values.Replace(value, value.WithIndentation());
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with the first element having a leading XML comment.
        /// </summary>
        /// <param name="values">
        /// The list whose first element is to be adjusted.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with the first element having a leading XML comment.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithLeadingXmlComment(this in SyntaxList<XmlNodeSyntax> values)
        {
            var value = values[0];

            return values.Replace(value, value.WithoutLeadingTrivia().WithLeadingXmlComment());
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with the first XML newline removed from the first element.
        /// </summary>
        /// <param name="values">
        /// The list whose first element is to be adjusted.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with the first XML newline removed, or the original list if no XML newline is present.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithoutFirstXmlNewLine(this in SyntaxList<XmlNodeSyntax> values)
        {
            if (values.FirstOrDefault() is XmlTextSyntax text)
            {
                var newText = text.WithoutFirstXmlNewLine();

                return newText.TextTokens.Count != 0
                       ? values.Replace(text, newText)
                       : values.Remove(text);
            }

            return values;
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with the leading trivia removed from the first element.
        /// </summary>
        /// <param name="values">
        /// The list whose first element is to be adjusted.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with the leading trivia removed from the first element.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithoutLeadingTrivia(this in SyntaxList<XmlNodeSyntax> values)
        {
            var value = values[0];

            return values.Replace(value, value.WithoutLeadingTrivia());
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with the leading XML comment removed from the first element.
        /// </summary>
        /// <param name="value">
        /// The list whose first element is to be adjusted.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with the leading XML comment removed, or the original list if no XML comment is present.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithoutLeadingXmlComment(this in SyntaxList<XmlNodeSyntax> value)
        {
            if (value.FirstOrDefault() is XmlTextSyntax text)
            {
                var replacement = text.WithoutLeadingXmlComment();

                // ensure that we have some text tokens
                if (replacement.TextTokens.Count > 0)
                {
                    return value.Replace(text, replacement.WithoutLeadingTrivia());
                }

                // remove text as no tokens remain
                return value.Remove(text);
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with all occurrences of the specified text removed from XML text elements.
        /// </summary>
        /// <param name="values">
        /// The list from which to remove the text.
        /// </param>
        /// <param name="text">
        /// The text to be removed.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with all occurrences of the text removed from XML text elements.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithoutText(this in SyntaxList<XmlNodeSyntax> values, string text)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var count = values.Count;

            if (count is 0)
            {
                return values;
            }

            var contents = values.ToList();

            for (var index = 0; index < count; index++)
            {
                if (values[index] is XmlTextSyntax s)
                {
                    var originalTextTokens = s.TextTokens;

                    // keep in local variable to avoid multiple requests (see Roslyn implementation)
                    var originalTextTokensCount = originalTextTokens.Count;

                    if (originalTextTokensCount is 0)
                    {
                        continue;
                    }

                    var textTokens = originalTextTokens.ToList();

                    var modified = false;

                    for (var i = 0; i < originalTextTokensCount; i++)
                    {
                        var token = originalTextTokens[i];

                        if (token.IsKind(SyntaxKind.XmlTextLiteralToken) && token.Text.Contains(text))
                        {
                            // do not trim the end as we want to have a space before <param> or other tags
                            var modifiedText = token.Text
                                                    .AsCachedBuilder()
                                                    .Without(text)
                                                    .WithoutMultipleWhiteSpaces()
                                                    .ToStringAndRelease();

                            if (modifiedText.IsNullOrWhiteSpace())
                            {
                                textTokens.Remove(token);

                                if (i > 0)
                                {
                                    textTokens.Remove(originalTextTokens[i - 1]);
                                }
                            }
                            else
                            {
                                textTokens[i] = token.WithText(modifiedText);
                            }

                            modified = true;
                        }
                    }

                    if (modified)
                    {
                        contents[index] = textTokens.AsXmlText();
                    }
                }
            }

            return contents.ToSyntaxList();
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with all occurrences of the specified texts removed from XML text elements.
        /// </summary>
        /// <param name="values">
        /// The list from which to remove the texts.
        /// </param>
        /// <param name="texts">
        /// The texts to be removed.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with all occurrences of the texts removed from XML text elements.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithoutText(this in SyntaxList<XmlNodeSyntax> values, in ReadOnlySpan<string> texts)
        {
            var length = texts.Length;

            if (length <= 0)
            {
                return values;
            }

            var result = values;

            for (var index = 0; index < length; index++)
            {
                result = result.WithoutText(texts[index]);
            }

            return result;
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with the trailing XML comment removed from the last element.
        /// </summary>
        /// <param name="value">
        /// The list whose last element is to be adjusted.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with the trailing XML comment removed, or the original list if no XML comment is present.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithoutTrailingXmlComment(this in SyntaxList<XmlNodeSyntax> value)
        {
            if (value.LastOrDefault() is XmlTextSyntax text)
            {
                var replacement = text.WithoutTrailingXmlComment();

                // ensure that we have some text tokens
                if (replacement.TextTokens.Count > 0)
                {
                    return value.Replace(text, replacement);
                }

                // remove text as no tokens remain
                return value.Remove(text);
            }

            return value;
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with the specified start texts removed from the first element.
        /// </summary>
        /// <param name="values">
        /// The list whose first element is to be adjusted.
        /// </param>
        /// <param name="startTexts">
        /// The start texts to be removed.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with the start texts removed from the first element, or the original list if no such text is present.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithoutStartText(this in SyntaxList<XmlNodeSyntax> values, in ReadOnlySpan<string> startTexts)
        {
            if (values.Count > 0 && values[0] is XmlTextSyntax textSyntax)
            {
                return values.Replace(textSyntax, textSyntax.WithoutStartText(startTexts));
            }

            return values;
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with the specified text prepended to the first element.
        /// </summary>
        /// <param name="values">
        /// The list whose first element is to be adjusted.
        /// </param>
        /// <param name="startText">
        /// The text to prepend.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the first word.
        /// The default is <see cref="FirstWordAdjustment.StartLowerCase"/>.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with the text prepended to the first element.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithStartText(this in SyntaxList<XmlNodeSyntax> values, string startText, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.StartLowerCase)
        {
            if (values.Count > 0)
            {
                return values[0] is XmlTextSyntax textSyntax
                       ? values.Replace(textSyntax, textSyntax.WithStartText(startText, firstWordAdjustment))
                       : values.Insert(0, startText.AsXmlText());
            }

            return startText.AsXmlText().ToSyntaxList<XmlNodeSyntax>();
        }

        /// <summary>
        /// Gets a <see cref="SyntaxList{T}"/> with the last element having a trailing XML comment.
        /// </summary>
        /// <param name="values">
        /// The list whose last element is to be adjusted.
        /// </param>
        /// <returns>
        /// A collection of XML node syntaxes with the last element having a trailing XML comment.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithTrailingXmlComment(this in SyntaxList<XmlNodeSyntax> values) => values.Replace(values.Last(), values.Last().WithoutTrailingTrivia().WithTrailingXmlComment());

        /// <summary>
        /// Retrieves the <see cref="XmlCrefAttributeSyntax"/> from the specified <see cref="SyntaxList{T}"/>.
        /// </summary>
        /// <param name="value">
        /// The list of attributes to search.
        /// </param>
        /// <returns>
        /// The <see cref="XmlCrefAttributeSyntax"/> if found; otherwise, <see langword="null"/>.
        /// </returns>
        internal static XmlCrefAttributeSyntax GetCref(this in SyntaxList<XmlAttributeSyntax> value)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            for (int index = 0, count = value.Count; index < count; index++)
            {
                if (value[index] is XmlCrefAttributeSyntax a)
                {
                    return a;
                }
            }

            return default;
        }
    }
}