﻿using System;
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
            var count = 0;

            foreach (var syntaxTrivia in value.Token.LeadingTrivia)
            {
                if (syntaxTrivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    if (count == 1)
                    {
                        return true;
                    }

                    count++;
                }
            }

            return false;
        }

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this XmlElementSyntax value)
        {
            return value?.ChildNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens).Where(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken)) ?? Enumerable.Empty<SyntaxToken>();
        }

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this IEnumerable<XmlElementSyntax> value)
        {
            return value?.SelectMany(_ => _.ChildNodes<XmlTextSyntax>()).SelectMany(_ => _.TextTokens).Where(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken)) ?? Enumerable.Empty<SyntaxToken>();
        }

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this IEnumerable<XmlTextSyntax> value)
        {
            return value?.SelectMany(_ => _.TextTokens).Where(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken)) ?? Enumerable.Empty<SyntaxToken>();
        }

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value)
        {
            return value?.DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens).Where(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken)) ?? Enumerable.Empty<SyntaxToken>();
        }

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value, Func<XmlTextSyntax, bool> descendantNodesFilter)
        {
            return value?.DescendantNodes(descendantNodesFilter).SelectMany(_ => _.TextTokens).Where(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken)) ?? Enumerable.Empty<SyntaxToken>();
        }

        public static IEnumerable<SyntaxToken> GetXmlTextTokens(this DocumentationCommentTriviaSyntax value, Func<XmlElementSyntax, bool> descendantNodesFilter)
        {
            return value?.DescendantNodes(descendantNodesFilter).GetXmlTextTokens() ?? Enumerable.Empty<SyntaxToken>();
        }
    }
}