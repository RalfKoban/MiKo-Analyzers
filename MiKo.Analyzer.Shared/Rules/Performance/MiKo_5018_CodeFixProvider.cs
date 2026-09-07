using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5018_CodeFixProvider)), Shared]
    public sealed class MiKo_5018_CodeFixProvider : PerformanceCodeFixProvider
    {
        private static readonly SyntaxKind[] LogicalConditions = { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };

        private static readonly SyntaxKind[] SpecialParents = { SyntaxKind.ArrowExpressionClause, SyntaxKind.ReturnStatement, SyntaxKind.IfStatement };

        public override string FixableDiagnosticId => "MiKo_5018";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BinaryExpressionSyntax>().FirstOrDefault();

        protected override async Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            if (syntax is BinaryExpressionSyntax binary)
            {
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                return GetUpdatedSyntax(binary, semanticModel);
            }

            return syntax;
        }

        private static BinaryExpressionSyntax GetUpdatedSyntax(BinaryExpressionSyntax binary, SemanticModel semanticModel)
        {
            var originalBinaryLeft = binary.Left;
            var originalBinaryRight = binary.Right;

            var updatedBinaryLeft = originalBinaryLeft;
            var updatedBinaryRight = originalBinaryRight;

            if (updatedBinaryRight is BinaryExpressionSyntax right && right.IsAnyKind(LogicalConditions))
            {
                updatedBinaryRight = GetUpdatedSyntax(right, semanticModel);
            }

            if (updatedBinaryLeft is ParenthesizedExpressionSyntax)
            {
                // ignore these
            }
            else if (updatedBinaryLeft is InvocationExpressionSyntax)
            {
                // ignore these
            }
            else if (updatedBinaryLeft is BinaryExpressionSyntax b1 && b1.Left is ConditionalAccessExpressionSyntax)
            {
                // skip these
                return binary;
            }
            else if (updatedBinaryLeft is BinaryExpressionSyntax b2 && b2.IsNullCheck())
            {
                // nothing to inverse
                return binary.ReplaceNode(originalBinaryRight, updatedBinaryRight);
            }
            else if (updatedBinaryLeft is BinaryExpressionSyntax b3)
            {
                // ignore these
            }
            else
            {
                if (originalBinaryLeft.IsValueType(semanticModel))
                {
                    // nothing to inverse
                    return binary.ReplaceNode(originalBinaryRight, updatedBinaryRight);
                }
            }

            var updatedSyntax = binary.ReplaceNodes(new[] { originalBinaryLeft, originalBinaryRight }, (original, rewritten) => ReferenceEquals(original, originalBinaryLeft) ? updatedBinaryRight.WithTriviaFrom(original) : updatedBinaryLeft.WithTriviaFrom(original));

            return binary.Parent.IsAnyKind(SpecialParents)
                   ? updatedSyntax.WithoutTrailingTrivia() // only remove trailing trivia if condition is direct child of 'if/return/arrow clause' so that semicolon fits
                   : updatedSyntax;
        }
    }
}