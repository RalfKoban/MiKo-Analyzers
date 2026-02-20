using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6040_CodeFixProvider)), Shared]
    public sealed class MiKo_6040_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6040";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case MemberAccessExpressionSyntax _:
                    case MemberBindingExpressionSyntax _:
                    case ConditionalAccessExpressionSyntax _:
                        return node;
                }
            }

            return null;
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedSyntaxRoot = GetUpdatedSyntaxRoot(root, syntax, issue);

            return Task.FromResult(updatedSyntaxRoot);
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

                case MemberBindingExpressionSyntax b:
                {
                    var spaces = GetProposedSpaces(issue);

                    return b.WithOperatorToken(b.OperatorToken.WithLeadingSpaces(spaces));
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

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, SyntaxNode syntax, Diagnostic issue)
        {
            // adjust the invocations
            switch (syntax)
            {
                case MemberAccessExpressionSyntax a when a.Parent is InvocationExpressionSyntax invocation:
                    return UpdateInvocation(invocation, root, GetProposedAdditionalSpaces(issue));

                case MemberBindingExpressionSyntax b when b.Parent is InvocationExpressionSyntax invocation:
                    return UpdateInvocation(invocation, root, GetProposedAdditionalSpaces(issue));

                default:
                    return root;
            }
        }

        private static SyntaxNode UpdateInvocation(InvocationExpressionSyntax invocation, SyntaxNode root, in int additionalSpaces)
        {
            var argumentList = invocation.ArgumentList;

            if (argumentList.Arguments.Count > 0)
            {
                var descendants = SelfAndDescendantsOnSeparateLines(argumentList);
                descendants.Remove(argumentList); // remove list to see if multiple other arguments are on different lines

                if (descendants.Count > 0)
                {
                    var updatedInvocation = invocation.WithAdditionalLeadingSpacesOnDescendants(descendants, additionalSpaces);

                    return root.ReplaceNode(invocation, updatedInvocation);
                }
            }

            return root;
        }
    }
}