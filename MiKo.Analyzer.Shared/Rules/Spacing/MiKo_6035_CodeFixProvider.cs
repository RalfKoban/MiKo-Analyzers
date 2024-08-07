using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6035_CodeFixProvider)), Shared]
    public sealed class MiKo_6035_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6035";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is InvocationExpressionSyntax invocation)
            {
                return invocation.WithArgumentList(invocation.ArgumentList.WithoutLeadingTrivia())
                                 .WithExpression(GetUpdatedExpressionPlacedOnSameLine(invocation.Expression).WithoutTrailingTrivia());
            }

            return syntax;
        }
    }
}