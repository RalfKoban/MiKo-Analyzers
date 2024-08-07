using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6037_CodeFixProvider)), Shared]
    public sealed class MiKo_6037_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6037";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is InvocationExpressionSyntax invocation)
            {
                return invocation.WithArgumentList(GetUpdatedArgumentList(invocation.ArgumentList))
                                 .WithExpression(GetUpdatedExpressionPlacedOnSameLine(invocation.Expression).WithoutTrailingTrivia());
            }

            return syntax;
        }

        private static ArgumentListSyntax GetUpdatedArgumentList(ArgumentListSyntax argumentList)
        {
            var argumentSyntax = argumentList.Arguments.First();

            return argumentList.WithArguments(argumentSyntax.WithoutTrivia().ToSeparatedSyntaxList())
                               .WithOpenParenToken(argumentList.OpenParenToken.WithoutTrivia())
                               .WithCloseParenToken(argumentList.CloseParenToken.WithoutLeadingTrivia());
        }
    }
}