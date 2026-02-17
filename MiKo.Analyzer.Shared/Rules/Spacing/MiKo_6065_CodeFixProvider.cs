using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6065_CodeFixProvider)), Shared]
    public sealed class MiKo_6065_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6065";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case MemberAccessExpressionSyntax _:
                        return node;

                    case MemberBindingExpressionSyntax _:
                        return node.Parent?.Parent; // we are interested in the ConditionalAccessExpressionSyntax that is the parent of the InvocationExpressionSyntax
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            switch (syntax)
            {
                case MemberAccessExpressionSyntax a:
                {
                    var spaces = GetProposedSpaces(issue);

                    return a.WithOperatorToken(a.OperatorToken.WithLeadingSpaces(spaces));
                }

                case ConditionalAccessExpressionSyntax c:
                {
                    var spaces = GetProposedSpaces(issue);

                    return c.WithOperatorToken(c.OperatorToken.WithLeadingSpaces(spaces));
                }

                default:
                    return syntax;
            }
        }
    }
}