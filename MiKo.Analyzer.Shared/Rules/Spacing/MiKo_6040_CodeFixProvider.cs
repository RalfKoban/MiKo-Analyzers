using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6040_CodeFixProvider)), Shared]
    public sealed class MiKo_6040_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer.Id;

        protected override string Title => Resources.MiKo_6040_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is InvocationExpressionSyntax invocation)
            {
                var topMostInvocation = invocation.Ancestors<InvocationExpressionSyntax>().LastOrDefault();

                if (topMostInvocation != null)
                {
                    var spaces = -1;

                    if (topMostInvocation.Expression is MemberAccessExpressionSyntax topMost)
                    {
                        spaces = topMost.OperatorToken.GetStartPosition().Character;
                    }

                    if (spaces > -1)
                    {
                        if (invocation.Expression is MemberAccessExpressionSyntax m)
                        {
                            return invocation.WithExpression(m.WithOperatorToken(m.OperatorToken.WithLeadingSpaces(spaces)));
                        }
                    }
                }
            }

            return syntax;
        }
    }
}