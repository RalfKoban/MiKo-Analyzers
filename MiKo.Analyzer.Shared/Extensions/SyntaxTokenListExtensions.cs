using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxTokenList"/>.
    /// </summary>
    internal static class SyntaxTokenListExtensions
    {
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
        /// Converts a list of syntax tokens to an <see cref="XmlTextSyntax"/> node.
        /// </summary>
        /// <param name="textTokens">
        /// The list of syntax tokens to convert.
        /// </param>
        /// <returns>
        /// An <see cref="XmlTextSyntax"/> node containing the syntax tokens.
        /// </returns>
        internal static XmlTextSyntax AsXmlText(this in SyntaxTokenList textTokens) => SyntaxFactory.XmlText(textTokens);

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
        /// Finds the first token in the <see cref="SyntaxTokenList"/> of the specified kind.
        /// </summary>
        /// <param name="value">
        /// The list of syntax tokens to search.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the kind of syntax token to find.
        /// </param>
        /// <returns>
        /// The first token of the specified kind, or the default value if no such token is found.
        /// </returns>
        internal static SyntaxToken First(this in SyntaxTokenList value, in SyntaxKind kind)
        {
            var tokens = value.OfKind(kind);

            return tokens.Count > 0 ? tokens[0] : default;
        }

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
        /// Builds a string representation of the syntax token list without any trivia.
        /// </summary>
        /// <param name="textTokens">
        /// The list of syntax tokens to process.
        /// </param>
        /// <param name="builder">
        /// The <see cref="StringBuilder"/> to append the text to.
        /// </param>
        /// <returns>
        /// The <see cref="StringBuilder"/> with the appended text.
        /// </returns>
        internal static StringBuilder GetTextWithoutTrivia(this in SyntaxTokenList textTokens, StringBuilder builder)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount > 0)
            {
                for (var index = 0; index < textTokensCount; index++)
                {
                    var token = textTokens[index];

                    if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                    {
                        continue;
                    }

                    builder.Append(token.ValueText);
                }
            }

            return builder;
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
        /// Retrieves all tokens of the specified kind from the <see cref="SyntaxTokenList"/>.
        /// </summary>
        /// <param name="source">
        /// The list of syntax tokens to filter.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the kind of syntax tokens to retrieve.
        /// </param>
        /// <returns>
        /// A read-only list of syntax tokens of the specified kind.
        /// </returns>
        internal static IReadOnlyList<SyntaxToken> OfKind(this in SyntaxTokenList source, in SyntaxKind kind)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount is 0)
            {
                return Array.Empty<SyntaxToken>();
            }

            List<SyntaxToken> results = null;

            for (var index = 0; index < sourceCount; index++)
            {
                var item = source[index];

                if (item.IsKind(kind))
                {
                    if (results is null)
                    {
                        results = new List<SyntaxToken>(1);
                    }

                    results.Add(item);
                }
            }

            return results ?? (IReadOnlyList<SyntaxToken>)Array.Empty<SyntaxToken>();
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

            for (var index = 0; index < sourceCount; index++)
            {
                result.Add(source[index]);
            }

            return result;
        }

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
        /// Removes a token with empty text value from the <see cref="SyntaxTokenList"/>.
        /// </summary>
        /// <param name="values">
        /// The list of syntax tokens to process.
        /// </param>
        /// <param name="token">
        /// The token to potentially remove if it has empty text.
        /// </param>
        /// <returns>
        /// A <see cref="SyntaxTokenList"/> without the empty text token if applicable; otherwise, the original list.
        /// </returns>
        internal static SyntaxTokenList WithoutEmptyText(this in SyntaxTokenList values, in SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.XmlTextLiteralToken) && token.ValueText.IsNullOrWhiteSpace())
            {
                return values.Remove(token);
            }

            return values;
        }

        /// <summary>
        /// Creates a new <see cref="SyntaxTokenList"/> without the XML new line token at the beginning.
        /// </summary>
        /// <param name="values">
        /// The list of syntax tokens to process.
        /// </param>
        /// <returns>
        /// A <see cref="SyntaxTokenList"/> without XML new line tokens and empty text at the beginning.
        /// </returns>
        internal static SyntaxTokenList WithoutFirstXmlNewLine(this in SyntaxTokenList values)
        {
            var tokens = values;

            if (tokens.Count > 0)
            {
                tokens = WithoutEmptyText(tokens, tokens[0]);
            }

            if (tokens.Count > 0)
            {
                tokens = WithoutNewLine(tokens, tokens[0]);
            }

            if (tokens.Count > 0)
            {
                tokens = WithoutEmptyText(tokens, tokens[0]);
            }

            return tokens;
        }

        /// <summary>
        /// Creates a new <see cref="SyntaxTokenList"/> without the XML new line token at the end.
        /// </summary>
        /// <param name="values">
        /// The list of syntax tokens to process.
        /// </param>
        /// <returns>
        /// A <see cref="SyntaxTokenList"/> without XML new line tokens and empty text at the end.
        /// </returns>
        internal static SyntaxTokenList WithoutLastXmlNewLine(this in SyntaxTokenList values)
        {
            var tokens = values;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var tokensCount = values.Count;

            if (tokensCount > 0)
            {
                tokens = WithoutEmptyText(tokens, tokens[tokensCount - 1]);

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                tokensCount = tokens.Count;
            }

            if (tokensCount > 0)
            {
                tokens = WithoutNewLine(tokens, tokens[tokensCount - 1]);
            }

            return tokens;
        }

        /// <summary>
        /// Removes a new line token from the <see cref="SyntaxTokenList"/>.
        /// </summary>
        /// <param name="values">
        /// The list of syntax tokens to process.
        /// </param>
        /// <param name="token">
        /// The token to potentially remove if it is a new line token.
        /// </param>
        /// <returns>
        /// A <see cref="SyntaxTokenList"/> without the new line token if applicable; otherwise, the original list.
        /// </returns>
        internal static SyntaxTokenList WithoutNewLine(this in SyntaxTokenList values, in SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
            {
                return values.Remove(token);
            }

            return values;
        }
    }
}