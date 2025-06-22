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
    internal static class SyntaxTriviaExtensions
    {
        internal static int GetPositionWithinStartLine(this in SyntaxTrivia value) => value.GetLocation().GetPositionWithinStartLine();

        internal static int GetStartingLine(this in SyntaxTrivia value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this in SyntaxTrivia value) => value.GetLocation().GetEndingLine();

        internal static LinePosition GetStartPosition(this in SyntaxTrivia value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this in SyntaxTrivia value) => value.GetLocation().GetEndPosition();

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsEndOfLine(this in SyntaxTrivia value) => value.IsKind(SyntaxKind.EndOfLineTrivia);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsComment(this in SyntaxTrivia value)
        {
            // we use 'RawKind' for performance reasons as most likely, we have single line comments
            // (SyntaxKind.MultiLineCommentTrivia is 1 higher than SyntaxKind.SingleLineCommentTrivia, so we include both)
            return (uint)(value.RawKind - (int)SyntaxKind.SingleLineCommentTrivia) <= 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsMultiLineComment(this in SyntaxTrivia value) => value.IsKind(SyntaxKind.MultiLineCommentTrivia);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsSingleLineComment(this in SyntaxTrivia value) => value.IsKind(SyntaxKind.SingleLineCommentTrivia);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsSpanningMultipleLines(this in SyntaxTrivia value) => value.Token.IsSpanningMultipleLines();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsWhiteSpace(this in SyntaxTrivia value) => value.IsKind(SyntaxKind.WhitespaceTrivia);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsKind(this in SyntaxTrivia value, in SyntaxKind kind) => value.RawKind == (int)kind;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsAnyKind(this in SyntaxTrivia value, ISet<SyntaxKind> kinds) => kinds.Contains(value.Kind());

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

        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this XmlElementSyntax value)
        {
            if (value is null)
            {
                return Array.Empty<SyntaxToken>();
            }

            return value.ChildNodes<XmlTextSyntax>().GetXmlTextTokens();
        }

        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this IEnumerable<XmlElementSyntax> value)
        {
            if (value is null)
            {
                return Array.Empty<SyntaxToken>();
            }

            return value.SelectMany(_ => _.GetXmlTextTokens());
        }

        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this XmlTextSyntax value)
        {
            if (value != null)
            {
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
        }

        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this IEnumerable<XmlTextSyntax> value)
        {
            if (value is null)
            {
                return Array.Empty<SyntaxToken>();
            }

            return value.SelectMany(_ => _.GetXmlTextTokens());
        }

        internal static IReadOnlyList<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value)
        {
            return GetXmlTextTokens(value, node => node.IsCode() is false); // skip code
        }

        internal static IReadOnlyList<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value, Func<XmlElementSyntax, bool> descendantNodesFilter)
        {
            return (IReadOnlyList<SyntaxToken>)value?.DescendantNodes(descendantNodesFilter).GetXmlTextTokens().ToList() ?? Array.Empty<SyntaxToken>();
        }

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
    }
}