using System.Collections.Generic;
using System.Composition;
using System.Linq;

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

        private static readonly SyntaxKind[] SpecialParentHandling = { SyntaxKind.ArrowExpressionClause, SyntaxKind.ReturnStatement, SyntaxKind.IfStatement };

        public override string FixableDiagnosticId => "MiKo_5018";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BinaryExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is BinaryExpressionSyntax binary)
            {
                var left = FindProblematicNode(binary.Left); // TODO RKN: left will be "values.Length == 8"
                var right = binary.Right;

                var updatedSyntax = binary.ReplaceNodes(new[] { left, right }, (original, rewritten) => ReferenceEquals(original, left) ? right.WithTriviaFrom(original) : left.WithTriviaFrom(original));

                return binary.Parent.IsAnyKind(SpecialParentHandling)
                       ? updatedSyntax.WithoutTrailingTrivia() // only remove trailing trivia if condition is direct child of 'if/return/arrow clause' so that semicolon fits
                       : updatedSyntax;
            }

            return syntax;
        }

        private static ExpressionSyntax FindProblematicNode(ExpressionSyntax expression)
        {
            var node = expression;

            while (true)
            {
                if (node is BinaryExpressionSyntax binary && binary.IsAnyKind(LogicalConditions))
                {
                    var right = binary.Right;

                    if (right is BinaryExpressionSyntax binaryRight && (binaryRight.Left is ElementAccessExpressionSyntax || binaryRight.Right is ElementAccessExpressionSyntax))
                    {
                        // we have some element access, so we need to replace the complete node
                        return expression;
                    }

                    node = right;
                }
                else
                {
                    return node;
                }
            }
        }
    }
}