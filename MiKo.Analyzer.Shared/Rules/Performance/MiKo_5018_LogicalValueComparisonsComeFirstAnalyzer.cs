using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5018_LogicalValueComparisonsComeFirstAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5018";

        private static readonly SyntaxKind[] LogicalConditions = { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };

        public MiKo_5018_LogicalValueComparisonsComeFirstAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLogicalCondition, LogicalConditions);

        private static bool IsNullCheck(ExpressionSyntax expression)
        {
            switch (expression.WithoutParenthesis())
            {
                case BinaryExpressionSyntax binary when binary.Left.IsKind(SyntaxKind.NullLiteralExpression) || binary.Right.IsKind(SyntaxKind.NullLiteralExpression):
                    return true;

                case IsPatternExpressionSyntax pattern when pattern.Pattern is UnaryPatternSyntax unary && unary.Pattern is ConstantPatternSyntax constant && constant.Expression.IsKind(SyntaxKind.NullLiteralExpression):
                    return true;
            }

            return false;
        }

        private static bool HasIssue(BinaryExpressionSyntax binary, SyntaxNodeAnalysisContext context)
        {
            if (binary.Right.WithoutParenthesis() is BinaryExpressionSyntax right && right.IsKind(SyntaxKind.EqualsExpression))
            {
                var semanticModel = context.SemanticModel;

                if (right.Left.GetTypeSymbol(semanticModel)?.IsValueType is true && right.Right.GetTypeSymbol(semanticModel)?.IsValueType is true)
                {
                    if (binary.Left.WithoutParenthesis() is ExpressionSyntax left)
                    {
                        if (IsNullCheck(left))
                        {
                            // do not report null checks
                            return false;
                        }

                        if (left is BinaryExpressionSyntax nested && nested.IsAnyKind(LogicalConditions) is false)
                        {
                            if (nested.Left.GetTypeSymbol(semanticModel)?.IsValueType is true && nested.Right.GetTypeSymbol(semanticModel)?.IsValueType is true)
                            {
                                // do not report checks on value types
                                return false;
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private void AnalyzeLogicalCondition(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax expression)
            {
                if (HasIssue(expression, context))
                {
                    context.ReportDiagnostic(Issue(expression));
                }
            }
        }
    }
}