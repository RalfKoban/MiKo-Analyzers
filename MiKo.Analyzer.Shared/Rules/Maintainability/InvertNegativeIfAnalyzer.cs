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
        private static readonly SyntaxKind[] LogicalExpressions = { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };

        protected InvertNegativeIfAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected static bool IsAnyNegative(SyntaxNode condition) => GetConditionParts(condition).Exists(IsNegative);

        protected static bool IsMainlyNegative(SyntaxNode condition)
        {
            var positiveConditions = 0;
            var negativeConditions = 0;

            foreach (var node in GetConditionParts(condition))
            {
                if (IsNegative(node))
                {
                    negativeConditions++;
                }
                else
                {
                    positiveConditions++;
                }
            }

            return negativeConditions > positiveConditions;
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);

        protected virtual IEnumerable<Diagnostic> AnalyzeIfStatement(IfStatementSyntax node, SyntaxNodeAnalysisContext context) => Enumerable.Empty<Diagnostic>();

        private static bool IsNegative(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.LogicalNotExpression:
                    return true;

                case SyntaxKind.IsPatternExpression:
                    return ((IsPatternExpressionSyntax)node).Pattern is ConstantPatternSyntax c && c.Expression.IsKind(SyntaxKind.FalseLiteralExpression);

                default:
                    return false;
            }
        }

        private static List<SyntaxNode> GetConditionParts(SyntaxNode condition)
        {
            var parts = new List<SyntaxNode>();

            GetConditionParts(condition, parts);

            return parts;
        }

        private static void GetConditionParts(SyntaxNode condition, ICollection<SyntaxNode> parts)
        {
            while (true)
            {
                switch (condition)
                {
                    case BinaryExpressionSyntax binary when binary.IsAnyKind(LogicalExpressions):
                    {
                        GetConditionParts(binary.Left, parts);

                        condition = binary.Right;
                        continue;
                    }

                    case ParenthesizedExpressionSyntax parenthesized:
                    {
                        condition = parenthesized.Expression;
                        continue;
                    }

                    default:
                    {
                        parts.Add(condition);
                        break;
                    }
                }

                break;
            }
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (IfStatementSyntax)context.Node;

            var issues = AnalyzeIfStatement(node, context);

            ReportDiagnostics(context, issues);
        }
    }
}