using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6062_CodeFixProvider)), Shared]
    public sealed class MiKo_6062_CodeFixProvider : IndentedSpacingCodeFixProvider<ExpressionSyntax>
    {
        public override string FixableDiagnosticId => "MiKo_6062";

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            if (syntax.Parent is InitializerExpressionSyntax c && c.IsKind(SyntaxKind.ComplexElementInitializerExpression))
            {
                var expressions = c.Expressions;

                if (expressions.Count is 2 && expressions[0] == syntax)
                {
                    var other = expressions[1];

                    if (syntax.IsOnSameLineAs(other) is false)
                    {
                        var spaces = GetProposedSpaces(issue);

                        // we have to adjust the neighboring element as well
                        var updatedRoot = root.ReplaceNode(other, other.WithLeadingSpaces(spaces));

                        return base.GetUpdatedSyntaxRootAsync(document, updatedRoot, syntax, annotationOfSyntax, issue, cancellationToken);
                    }
                }
            }

            return base.GetUpdatedSyntaxRootAsync(document, root, syntax, annotationOfSyntax, issue, cancellationToken);
        }
    }
}