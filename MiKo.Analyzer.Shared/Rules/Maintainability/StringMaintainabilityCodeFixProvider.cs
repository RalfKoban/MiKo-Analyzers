using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class StringMaintainabilityCodeFixProvider : MaintainabilityCodeFixProvider
    {
        // TODO: RKN Nove to SyntaxNodeExtensions
        protected static SyntaxNode GetUpdatedSyntaxWithTextEnding(SyntaxNode syntax, string ending)
        {
            switch (syntax)
            {
                case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                    return literal.WithToken(literal.Token.WithText($"\"{GetFixedText(literal.Token, ending)}\""));

                case InterpolatedStringExpressionSyntax interpolated: // it's no text at the end, so add some
                    return interpolated.AddContents(SyntaxFactory.InterpolatedStringText(ending.ToSyntaxToken(SyntaxKind.InterpolatedStringTextToken)));

                case InterpolatedStringTextSyntax text:
                    return text.WithTextToken(text.TextToken.WithText(GetFixedText(text.TextToken, ending)));

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

        private static string GetFixedText(SyntaxToken token, string ending) => GetFixedText(token.ValueText, ending);

        private static string GetFixedText(string text, string ending) => GetCleanedText(text.TrimEnd()) + ending;

        private static string GetCleanedText(string text)
        {
            var lastCharIndex = text.Length - 1;

            if (lastCharIndex >= 0 && Constants.TrailingSentenceMarkers.Any(_ => _ == text[lastCharIndex]))
            {
                return text.Substring(0, lastCharIndex);
            }

            return text;
        }
    }
}