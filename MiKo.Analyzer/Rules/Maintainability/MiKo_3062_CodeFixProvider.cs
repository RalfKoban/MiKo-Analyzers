using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3062_CodeFixProvider)), Shared]
    public sealed class MiKo_3062_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        private const char FinalTextChar = ':';

        public override string FixableDiagnosticId => MiKo_3062_ExceptionLogMessageEndsWithColonAnalyzer.Id;

        protected override string Title => Resources.MiKo_3062_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            if (syntax is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                var token = literal.Token;

                return literal.WithToken(token.WithText($"\"{token.ValueText}{FinalTextChar}\""));
            }

            if (syntax is InterpolatedStringExpressionSyntax interpolated)
            {
                // it's no text at the end, so add some
                return interpolated.AddContents(SyntaxFactory.InterpolatedStringText(FinalTextChar.ToString().ToSyntaxToken(SyntaxKind.InterpolatedStringTextToken)));
            }

            if (syntax is InterpolatedStringTextSyntax text)
            {
                var token = text.TextToken;
                return text.WithTextToken(token.WithText(token.ValueText + FinalTextChar));
            }

            return syntax;
        }
    }
}