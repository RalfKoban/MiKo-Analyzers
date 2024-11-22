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
        internal static int GetPositionWithinStartLine(this SyntaxTrivia value) => value.GetLocation().GetPositionWithinStartLine();

        internal static int GetStartingLine(this SyntaxTrivia value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this SyntaxTrivia value) => value.GetLocation().GetEndingLine();

        internal static LinePosition GetStartPosition(this SyntaxTrivia value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this SyntaxTrivia value) => value.GetLocation().GetEndPosition();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsEndOfLine(this SyntaxTrivia value) => value.IsKind(SyntaxKind.EndOfLineTrivia);

        internal static bool IsComment(this SyntaxTrivia value)
        {
            switch (value.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    return true;

                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsMultiLineComment(this SyntaxTrivia value) => value.IsKind(SyntaxKind.MultiLineCommentTrivia);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsSingleLineComment(this SyntaxTrivia value) => value.IsKind(SyntaxKind.SingleLineCommentTrivia);

        internal static bool IsSpanningMultipleLines(this SyntaxTrivia value) => value.Token.IsSpanningMultipleLines();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsWhiteSpace(this SyntaxTrivia value) => value.IsKind(SyntaxKind.WhitespaceTrivia);

        internal static bool IsAnyKind(this SyntaxTrivia value, ISet<SyntaxKind> kinds) => kinds.Contains(value.Kind());

        internal static bool IsAnyKind(this SyntaxTrivia value, params SyntaxKind[] kinds)
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
                return Enumerable.Empty<SyntaxToken>();
            }

            return value.ChildNodes<XmlTextSyntax>().GetXmlTextTokens();
        }

        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this IEnumerable<XmlElementSyntax> value)
        {
            if (value is null)
            {
                return Enumerable.Empty<SyntaxToken>();
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
                return Enumerable.Empty<SyntaxToken>();
            }

            return value.SelectMany(_ => _.GetXmlTextTokens());
        }

        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value)
        {
            return GetXmlTextTokens(value, node => node.IsCode() is false); // skip code
        }

        internal static IEnumerable<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value, Func<XmlElementSyntax, bool> descendantNodesFilter)
        {
            return value?.DescendantNodes(descendantNodesFilter).GetXmlTextTokens() ?? Enumerable.Empty<SyntaxToken>();
        }

        internal static IEnumerable<SyntaxTrivia> NextSiblings(this SyntaxTrivia value, int count = int.MaxValue)
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

            return Enumerable.Empty<SyntaxTrivia>();
        }

        internal static IEnumerable<SyntaxTrivia> PreviousSiblings(this SyntaxTrivia value, int count = int.MaxValue)
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

            return Enumerable.Empty<SyntaxTrivia>();
        }
    }
}