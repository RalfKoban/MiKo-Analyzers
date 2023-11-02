using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class SyntaxTriviaExtensions
    {
        public static bool IsSpanningMultipleLines(this SyntaxTrivia value)
        {
            var foundLine = false;

            var leadingTrivia = value.Token.LeadingTrivia;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var count = leadingTrivia.Count;

            for (var index = 0; index < count; index++)
            {
                if (leadingTrivia[index].IsComment())
                {
                    if (foundLine)
                    {
                        return true;
                    }

                    foundLine = true;
                }
            }

            return false;
        }

        public static bool IsEndOfLine(this SyntaxTrivia value) => value.IsKind(SyntaxKind.EndOfLineTrivia);

        public static bool IsWhiteSpace(this SyntaxTrivia value) => value.IsKind(SyntaxKind.WhitespaceTrivia);

        public static bool IsSingleLineComment(this SyntaxTrivia value) => value.IsKind(SyntaxKind.SingleLineCommentTrivia);

        public static bool IsMultiLineComment(this SyntaxTrivia value) => value.IsKind(SyntaxKind.MultiLineCommentTrivia);

        public static bool IsComment(this SyntaxTrivia value)
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

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this XmlElementSyntax value)
        {
            if (value is null)
            {
                return Enumerable.Empty<SyntaxToken>();
            }

            return value.ChildNodes<XmlTextSyntax>().GetXmlTextTokens();
        }

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this IEnumerable<XmlElementSyntax> value)
        {
            if (value is null)
            {
                return Enumerable.Empty<SyntaxToken>();
            }

            return value.SelectMany(_ => _.GetXmlTextTokens());
        }

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this XmlTextSyntax value)
        {
            if (value != null)
            {
                var textTokens = value.TextTokens;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var tokensCount = textTokens.Count;

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

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this IEnumerable<XmlTextSyntax> value)
        {
            if (value is null)
            {
                return Enumerable.Empty<SyntaxToken>();
            }

            return value.SelectMany(_ => _.GetXmlTextTokens());
        }

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value)
        {
            return GetXmlTextTokens(value, node => node.IsCode() is false); // skip code
        }

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value, Func<XmlElementSyntax, bool> descendantNodesFilter)
        {
            return value?.DescendantNodes(descendantNodesFilter).GetXmlTextTokens() ?? Enumerable.Empty<SyntaxToken>();
        }
    }
}