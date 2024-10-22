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
                case BinaryExpressionSyntax binary when IsNullCheck(binary):
                    return true;

                case IsPatternExpressionSyntax pattern when pattern.Pattern is UnaryPatternSyntax unary && unary.Pattern is ConstantPatternSyntax constant && constant.Expression.IsKind(SyntaxKind.NullLiteralExpression):
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsNullCheck(BinaryExpressionSyntax expression) => expression.Left.IsKind(SyntaxKind.NullLiteralExpression) || expression.Right.IsKind(SyntaxKind.NullLiteralExpression);

        private static bool HasIssue(BinaryExpressionSyntax binary, SyntaxNodeAnalysisContext context)
        {
            if (binary.Right.WithoutParenthesis() is BinaryExpressionSyntax right && right.IsKind(SyntaxKind.EqualsExpression))
            {
                if (binary.Left.WithoutParenthesis() is ExpressionSyntax left)
                {
                    if (IsNullCheck(left))
                    {
                        // do not report null checks
                        return false;
                    }

                    switch (left)
                    {
                        case IdentifierNameSyntax _: return false; // do not report checks on boolean members
                        case IsPatternExpressionSyntax e when e.Pattern is DeclarationPatternSyntax: return false; // do not report pattern checks
                        case BinaryExpressionSyntax nested:
                        {
                            if (nested.IsAnyKind(LogicalConditions))
                            {
                                if (IsNullCheck(nested.Right))
                                {
                                    return false; // do not report nested null checks
                                }
                            }
                            else
                            {
                                // no logical condition
                                var nestedLeft = nested.Left;
                                var nestedRight = nested.Right;

                                if (nestedLeft is InvocationExpressionSyntax || nestedRight is InvocationExpressionSyntax)
                                {
                                    // report on method calls
                                    return true;
                                }

                                if (nestedLeft.IsKind(SyntaxKind.StringLiteralExpression) || nestedRight.IsKind(SyntaxKind.StringLiteralExpression))
                                {
                                    // do not report checks on strings and value types
                                    return false;
                                }

                                if (nestedLeft.IsKind(SyntaxKind.NumericLiteralExpression) || nestedRight.IsKind(SyntaxKind.NumericLiteralExpression))
                                {
                                    // do not report checks on value types
                                    return false;
                                }

                                if (nestedLeft is MemberAccessExpressionSyntax || nestedRight is MemberAccessExpressionSyntax)
                                {
                                    // do not report on members
                                    return false;
                                }
                            }

                            break;
                        }
                    }
                }

                if (right.Left is ElementAccessExpressionSyntax && right.Right is LiteralExpressionSyntax)
                {
                    // do not report on element access
                    return false;
                }

                var semanticModel = context.SemanticModel;

                if (right.Left.GetTypeSymbol(semanticModel)?.IsValueType is true && right.Right.GetTypeSymbol(semanticModel)?.IsValueType is true)
                {
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