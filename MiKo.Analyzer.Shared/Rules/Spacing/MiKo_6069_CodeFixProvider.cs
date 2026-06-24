using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6069_CodeFixProvider)), Shared]
    public sealed class MiKo_6069_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6069";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<AssignmentExpressionSyntax>().FirstOrDefault();

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntaxRoot(root, syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is AssignmentExpressionSyntax expression)
            {
                var spaces = GetProposedSpaces(issue);

                return expression.WithoutLeadingTrivia().WithLeadingSpaces(spaces);
            }

            return syntax;
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root, SyntaxNode syntax)
        {
            if (syntax is AssignmentExpressionSyntax a && a.Parent is InitializerExpressionSyntax initializer)
            {
                var openBraceToken = initializer.OpenBraceToken;

                if (initializer.CloseBraceToken.IsOnSameLineAs(openBraceToken))
                {
                    var expressions = initializer.Expressions;
                    var index = expressions.IndexOf(a);

                    switch (index)
                    {
                        case -1: // should never happen as the syntax should be part of the initializer
                            break;

                        case 0:
                        {
                            // ensure that the open brace token has no trailing trivia
                            if (openBraceToken.HasTrailingTrivia)
                            {
                                return root.ReplaceNode(initializer, initializer.WithOpenBraceToken(openBraceToken.WithoutTrailingTrivia()));
                            }

                            break;
                        }

                        default:
                        {
                            var separator = expressions.GetSeparator(index - 1);

                            // ensure that the separator token has no trailing trivia
                            if (separator.HasTrailingTrivia)
                            {
                                return root.ReplaceToken(separator, separator.WithoutTrailingTrivia());
                            }

                            break;
                        }
                    }
                }
            }

            return root;
        }
    }
}