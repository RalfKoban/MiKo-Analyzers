using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class InvertNegativeIfAnalyzer : MaintainabilityAnalyzer
    {
        private static readonly SyntaxKind[] BinaryConditions = { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };

        protected InvertNegativeIfAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected static bool IsNegative(SyntaxNode condition)
        {
            var positiveConditions = 0;
            var negativeConditions = 0;

            foreach (var node in condition.DescendantNodesAndSelf())
            {
                if (IsNegativeCore(node))
                {
                    negativeConditions++;
                }
                else
                {
                    if (node is BinaryExpressionSyntax binary && binary.IsAnyKind(BinaryConditions) && IsNegativeCore(binary.Left) is false && IsNegativeCore(binary.Right) is false)
                    {
                        positiveConditions++;
                    }
                }
            }

            return negativeConditions > positiveConditions;

            bool IsNegativeCore(SyntaxNode node) => node.IsKind(SyntaxKind.LogicalNotExpression) || (node is IsPatternExpressionSyntax pattern && pattern.Pattern is ConstantPatternSyntax c && c.Expression.IsKind(SyntaxKind.FalseLiteralExpression));
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);

        protected virtual IEnumerable<Diagnostic> AnalyzeIfStatement(IfStatementSyntax node, SyntaxNodeAnalysisContext context) => Enumerable.Empty<Diagnostic>();

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (IfStatementSyntax)context.Node;

            var issues = AnalyzeIfStatement(node, context);

            ReportDiagnostics(context, issues);
        }
    }
}