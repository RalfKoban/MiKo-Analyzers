using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Extensions
{
    internal static class SyntaxTokenExtensions
    {
        internal static SyntaxToken WithText(this SyntaxToken token, string text) => SyntaxFactory.Token(token.LeadingTrivia, token.Kind(), text, text, token.TrailingTrivia);

        internal static SyntaxToken ToSyntaxToken(this string text, SyntaxKind kind = SyntaxKind.StringLiteralToken) => SyntaxFactory.Token(default, kind, text, text, default);
    }
}