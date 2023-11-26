using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class StringMaintainabilityCodeFixProvider : MaintainabilityCodeFixProvider
    {
        protected static SyntaxNode GetUpdatedSyntaxWithTextEnding(SyntaxNode syntax, string ending)
        {
            switch (syntax)
            {
                case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                    return literal.WithToken(literal.Token.WithText(GetFixedText(literal.Token).SurroundedWithDoubleQuote()));

                case InterpolatedStringExpressionSyntax interpolated: // it's no text at the end, so add some
                    return interpolated.AddContents(SyntaxFactory.InterpolatedStringText(ending.AsToken(SyntaxKind.InterpolatedStringTextToken)));

                case InterpolatedStringTextSyntax text:
                    return text.WithTextToken(text.TextToken.WithText(GetFixedText(text.TextToken)));

                default:
                    return syntax;
            }

            string GetFixedText(SyntaxToken token) => GetCleanedText(token.ValueText.AsSpan().TrimEnd()).ToString() + ending;

            ReadOnlySpan<char> GetCleanedText(ReadOnlySpan<char> text)
            {
                var lastCharIndex = text.Length - 1;

                if (lastCharIndex >= 0)
                {
                    foreach (var marker in Constants.TrailingSentenceMarkers)
                    {
                        if (marker == text[lastCharIndex])
                        {
                            return text.Slice(0, lastCharIndex);
                        }
                    }
                }

                return text;
            }
        }

        protected static SyntaxNode GetUpdatedSyntaxWithFixedText(SyntaxNode syntax, Func<string, string> callback)
        {
            switch (syntax)
            {
                case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                {
                    return literal.WithToken(literal.Token.WithText(callback(literal.Token.ValueText).SurroundedWithDoubleQuote()));
                }

                case InterpolatedStringTextSyntax text:
                {
                    return text.WithTextToken(text.TextToken.WithText(callback(text.TextToken.ValueText)));
                }

                default:
                    return syntax;
            }
        }

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            var node = syntaxNodes.First();

            switch (node)
            {
                case InterpolationSyntax i when i.Parent is InterpolatedStringExpressionSyntax interpolated:
                    return interpolated;

                case IdentifierNameSyntax name when name.Parent is InterpolationSyntax i && i.Parent is InterpolatedStringExpressionSyntax interpolated:
                    return interpolated;

                default:
                    return node;
            }
        }
    }
}