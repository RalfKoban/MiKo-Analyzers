using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxTrivia"/>.
    /// </summary>
    internal static class SyntaxTriviaExtensions
    {
        /// <summary>
        /// Gets the line span information for a syntax trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia to get the line span for.
        /// </param>
        /// <returns>
        /// A file line position span representing the line span information.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static FileLinePositionSpan GetLineSpan(this in SyntaxTrivia value) => value.GetLocation().GetLineSpan();

        /// <summary>
        /// Gets the character position within the starting line of a syntax trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia to get the position for.
        /// </param>
        /// <returns>
        /// An integer representing the character position within the starting line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPositionWithinStartLine(this in SyntaxTrivia value) => value.GetLocation().GetPositionWithinStartLine();

        /// <summary>
        /// Gets the starting line number of a syntax trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia to get the starting line for.
        /// </param>
        /// <returns>
        /// An integer representing the starting line number.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetStartingLine(this in SyntaxTrivia value) => value.GetLocation().GetStartingLine();

        /// <summary>
        /// Gets the ending line number of a syntax trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia to get the ending line for.
        /// </param>
        /// <returns>
        /// An integer representing the ending line number.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetEndingLine(this in SyntaxTrivia value) => value.GetLocation().GetEndingLine();

        /// <summary>
        /// Gets the starting position of a syntax trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia to get the starting position for.
        /// </param>
        /// <returns>
        /// A line position representing the starting position.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static LinePosition GetStartPosition(this in SyntaxTrivia value) => value.GetLocation().GetStartPosition();

        /// <summary>
        /// Gets the ending position of a syntax trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia to get the ending position for.
        /// </param>
        /// <returns>
        /// A line position representing the ending position.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static LinePosition GetEndPosition(this in SyntaxTrivia value) => value.GetLocation().GetEndPosition();

        /// <summary>
        /// Determines whether the trivia list contains any end of line trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia list to examine.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the list contains any end of line trivia; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasEndOfLine(this in SyntaxTriviaList value)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            if (valueCount > 0)
            {
                for (var index = 0; index < valueCount; index++)
                {
                    if (value[index].IsEndOfLine())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the trivia list contains any comment trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia list to examine.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the list contains any comment trivia; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasComment(this in SyntaxTriviaList value)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valueCount = value.Count;

            if (valueCount > 0)
            {
                for (var index = 0; index < valueCount; index++)
                {
                    if (value[index].IsComment())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the trivia is an end of line trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia to examine.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the trivia is an end of line trivia; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsEndOfLine(this in SyntaxTrivia value) => value.IsKind(SyntaxKind.EndOfLineTrivia);

        /// <summary>
        /// Determines whether the trivia is a comment trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia to examine.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the trivia is a single line or multi-line comment; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsComment(this in SyntaxTrivia value)
        {
            // we use 'RawKind' for performance reasons as most likely, we have single line comments
            // (SyntaxKind.MultiLineCommentTrivia is 1 higher than SyntaxKind.SingleLineCommentTrivia, so we include both)
            return (uint)(value.RawKind - (int)SyntaxKind.SingleLineCommentTrivia) <= 1;
        }

        /// <summary>
        /// Determines whether the trivia is a multi-line comment.
        /// </summary>
        /// <param name="value">
        /// The trivia to examine.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the trivia is a multi-line comment; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsMultiLineComment(this in SyntaxTrivia value) => value.IsKind(SyntaxKind.MultiLineCommentTrivia);

        /// <summary>
        /// Determines whether the trivia is a single line comment.
        /// </summary>
        /// <param name="value">
        /// The trivia to examine.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the trivia is a single line comment; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsSingleLineComment(this in SyntaxTrivia value) => value.IsKind(SyntaxKind.SingleLineCommentTrivia);

        /// <summary>
        /// Determines whether the trivia spans multiple lines.
        /// </summary>
        /// <param name="value">
        /// The trivia to examine.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the trivia spans multiple lines; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsSpanningMultipleLines(this in SyntaxTrivia value) => value.Token.IsSpanningMultipleLines();

        /// <summary>
        /// Determines whether the trivia is a whitespace.
        /// </summary>
        /// <param name="value">
        /// The trivia to examine.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the trivia is a whitespace; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsWhiteSpace(this in SyntaxTrivia value) => value.IsKind(SyntaxKind.WhitespaceTrivia);

        /// <summary>
        /// Determines whether the trivia is of the specified syntax kind.
        /// </summary>
        /// <param name="value">
        /// The trivia to examine.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the syntax kind to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the trivia is of the specified kind; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsKind(this in SyntaxTrivia value, in SyntaxKind kind) => value.RawKind == (int)kind;

        /// <summary>
        /// Determines whether the trivia is of the specified syntax kinds.
        /// </summary>
        /// <param name="value">
        /// The trivia to examine.
        /// </param>
        /// <param name="kinds">
        /// The set of syntax kinds to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the trivia is of the specified kinds; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsAnyKind(this in SyntaxTrivia value, ISet<SyntaxKind> kinds) => kinds.Contains(value.Kind());

        /// <summary>
        /// Determines whether the trivia is of the specified syntax kinds.
        /// </summary>
        /// <param name="value">
        /// The trivia to examine.
        /// </param>
        /// <param name="kinds">
        /// The span of syntax kinds to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the trivia is of the specified kinds; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsAnyKind(this in SyntaxTrivia value, in ReadOnlySpan<SyntaxKind> kinds)
        {
            var valueKind = value.Kind();

            // for performance reasons we use indexing instead of an enumerator
            var length = kinds.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    if (kinds[index] == valueKind)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets XML text tokens from an XML element.
        /// </summary>
        /// <param name="value">
        /// The XML element to get tokens from.
        /// </param>
        /// <returns>
        /// A collection of syntax tokens from the XML element.
        /// </returns>
        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this XmlElementSyntax value)
        {
            if (value is null)
            {
                return Array.Empty<SyntaxToken>();
            }

            return value.ChildNodes<XmlTextSyntax>().GetXmlTextTokens();
        }

        /// <summary>
        /// Gets XML text tokens from a collection of XML elements.
        /// </summary>
        /// <param name="value">
        /// The collection of XML elements to get tokens from.
        /// </param>
        /// <returns>
        /// A collection of syntax tokens from all XML elements.
        /// </returns>
        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this IEnumerable<XmlElementSyntax> value)
        {
            if (value is null)
            {
                return Array.Empty<SyntaxToken>();
            }

            return GetXmlTextTokensLocal(value);

            IEnumerable<SyntaxToken> GetXmlTextTokensLocal(IEnumerable<XmlElementSyntax> elements)
            {
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var element in elements)
                {
                    foreach (var token in element.GetXmlTextTokens())
                    {
                        yield return token;
                    }
                }
            }
        }

        /// <summary>
        /// Gets XML text tokens from an XML text syntax node.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax node to get tokens from.
        /// </param>
        /// <returns>
        /// A collection of non-empty syntax tokens from the XML text.
        /// </returns>
        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this XmlTextSyntax value)
        {
            if (value is null)
            {
                yield break;
            }

            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var tokensCount = textTokens.Count;

            if (tokensCount > 0)
            {
                for (var index = 0; index < tokensCount; index++)
                {
                    var token = textTokens[index];

                    if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                    {
                        var text = token.ValueText;

                        if (text.Length <= Constants.MinimumCharactersThreshold && string.IsNullOrWhiteSpace(text))
                        {
                            // nothing to inspect as the text is too short and consists of whitespaces only
                            continue;
                        }

                        yield return token;
                    }
                }
            }
        }

        /// <summary>
        /// Gets XML text tokens from a collection of XML text syntax nodes.
        /// </summary>
        /// <param name="value">
        /// The collection of XML text syntax nodes to get tokens from.
        /// </param>
        /// <returns>
        /// A collection of syntax tokens from all XML text nodes.
        /// </returns>
        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this IEnumerable<XmlTextSyntax> value)
        {
            if (value is null)
            {
                return Array.Empty<SyntaxToken>();
            }

            return GetXmlTextTokensLocal(value);

            IEnumerable<SyntaxToken> GetXmlTextTokensLocal(IEnumerable<XmlTextSyntax> nodes)
            {
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var node in nodes)
                {
                    foreach (var token in node.GetXmlTextTokens())
                    {
                        yield return token;
                    }
                }
            }
        }

        /// <summary>
        /// Gets XML text tokens from a documentation comment trivia, excluding code blocks.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia to get tokens from.
        /// </param>
        /// <returns>
        /// A collection of syntax tokens that contains syntax tokens from the documentation comment.
        /// </returns>
        internal static IReadOnlyList<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value)
        {
            return GetXmlTextTokens(value, node => node.IsCode() is false); // skip code
        }

        /// <summary>
        /// Gets XML text tokens from a documentation comment trivia using a custom filter.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia to get tokens from.
        /// </param>
        /// <param name="descendantNodesFilter">
        /// A callback to filter which descendant nodes to include.
        /// </param>
        /// <returns>
        /// A collection of syntax tokens that contains syntax tokens from the filtered documentation comment nodes.
        /// </returns>
        internal static IReadOnlyList<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value, Func<XmlElementSyntax, bool> descendantNodesFilter)
        {
            if (value is null)
            {
                return Array.Empty<SyntaxToken>();
            }

            var tokens = value?.DescendantNodes(descendantNodesFilter).GetXmlTextTokens();

            if (tokens is SyntaxToken[] array)
            {
                return array;
            }

            return tokens.ToList();
        }

        /// <summary>
        /// Gets the next sibling trivia of the specified trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia to get the next siblings for.
        /// </param>
        /// <param name="count">
        /// The maximum number of siblings to return.
        /// The default is <see cref="int.MaxValue"/>.
        /// </param>
        /// <returns>
        /// A collection of the next sibling trivia.
        /// </returns>
        internal static IEnumerable<SyntaxTrivia> NextSiblings(this in SyntaxTrivia value, in int count = int.MaxValue)
        {
            if (count > 0)
            {
                var parent = value.Token;

                var leadingTrivia = parent.LeadingTrivia;
                var nextLeadingIndex = leadingTrivia.IndexOf(value) + 1;

                if (nextLeadingIndex > 0)
                {
                    return leadingTrivia.Skip(nextLeadingIndex).Take(count);
                }

                var trailingTrivia = parent.TrailingTrivia;
                var nextTrailingIndex = trailingTrivia.IndexOf(value) + 1;

                if (nextTrailingIndex > 0)
                {
                    return trailingTrivia.Skip(nextTrailingIndex).Take(count);
                }
            }

            return Array.Empty<SyntaxTrivia>();
        }

        /// <summary>
        /// Gets the previous sibling trivia of the specified trivia.
        /// </summary>
        /// <param name="value">
        /// The trivia to get the previous siblings for.
        /// </param>
        /// <param name="count">
        /// The maximum number of siblings to return.
        /// The default is <see cref="int.MaxValue"/>.
        /// </param>
        /// <returns>
        /// A collection of the previous sibling trivia.
        /// </returns>
        internal static IEnumerable<SyntaxTrivia> PreviousSiblings(this in SyntaxTrivia value, in int count = int.MaxValue)
        {
            if (count > 0)
            {
                var parent = value.Token;

                var leadingTrivia = parent.LeadingTrivia;
                var index = leadingTrivia.IndexOf(value);

                if (index >= 0)
                {
                    return leadingTrivia.Take(index).Reverse().Take(count).Reverse();
                }

                var trailingTrivia = parent.TrailingTrivia;
                index = trailingTrivia.IndexOf(value);

                if (index >= 0)
                {
                    return trailingTrivia.Take(index).Reverse().Take(count).Reverse();
                }
            }

            return Array.Empty<SyntaxTrivia>();
        }

        /// <summary>
        /// Converts a syntax trivia to a text-only string, removing comment markers and whitespace.
        /// </summary>
        /// <param name="source">
        /// The trivia to convert to a text-only string.
        /// </param>
        /// <returns>
        /// A string containing only the text content of the trivia, or the original trivia string if not a comment.
        /// </returns>
        internal static string ToTextOnlyString(this in SyntaxTrivia source)
        {
            if (source.IsComment() && source.SyntaxTree is SyntaxTree tree)
            {
                var sourceText = tree.GetText();

                var span = source.Span;
                var start = span.Start + 2; // the text '//' has 2 characters
                var end = span.End;

                while (sourceText[start].IsWhiteSpace())
                {
                    start++;
                }

                while (sourceText[end].IsWhiteSpace())
                {
                    end--;
                }

                if (start >= end)
                {
                    return string.Empty;
                }

                return sourceText.ToString(TextSpan.FromBounds(start, end + 1));
            }

            return source.ToString();
        }
    }
}