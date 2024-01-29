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
        protected InvertNegativeIfAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected static bool IsNegative(SyntaxNode condition)
        {
            foreach (var node in condition.DescendantNodesAndSelf())
            {
                if (node.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    return true;
                }

                if (node is IsPatternExpressionSyntax pattern && pattern.Pattern is ConstantPatternSyntax c && c.Expression.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    return true;
                }
            }

            return false;
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