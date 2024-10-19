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

        public override string FixableDiagnosticId => "MiKo_5018";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<BinaryExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is BinaryExpressionSyntax binary)
            {
                var left = FindProblematicNode(binary.Left);
                var right = binary.Right;

                return binary.ReplaceNodes(new[] { left, right }, (original, rewritten) => ReferenceEquals(original, left) ? right.WithTrailingSpace() : left.WithoutTrivia());
            }

            return syntax;
        }

        private static ExpressionSyntax FindProblematicNode(ExpressionSyntax expression)
        {
            while (true)
            {
                if (expression is BinaryExpressionSyntax binary && binary.IsAnyKind(LogicalConditions))
                {
                    expression = binary.Right;
                }
                else
                {
                    return expression;
                }
            }
        }
    }
}