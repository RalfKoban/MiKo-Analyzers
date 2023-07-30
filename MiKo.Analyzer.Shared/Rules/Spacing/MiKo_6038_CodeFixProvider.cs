using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6038_CodeFixProvider)), Shared]
    public sealed class MiKo_6038_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6038_CastsAreOnSameLineAnalyzer.Id;

        protected override string Title => Resources.MiKo_6038_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<CastExpressionSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is CastExpressionSyntax cast)
            {
                return cast.WithOpenParenToken(cast.OpenParenToken.WithoutTrivia())
                           .WithType(cast.Type.WithoutTrivia())
                           .WithCloseParenToken(cast.CloseParenToken.WithoutTrivia())
                           .WithExpression(cast.Expression.WithoutLeadingTrivia());
            }

            return syntax;
        }
    }
}