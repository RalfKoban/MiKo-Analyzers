using System.Collections.Generic;
using System.Linq;

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

        private static bool IsNullCheck(BinaryExpressionSyntax expression) => expression.Left.IsKind(SyntaxKind.NullLiteralExpression) || expression.Right.IsKind(SyntaxKind.NullLiteralExpression);

        private static bool IsNullCheck(IsPatternExpressionSyntax expression)
        {
            switch (expression.Pattern)
            {
                case ConstantPatternSyntax constant when constant.Expression.IsKind(SyntaxKind.NullLiteralExpression): return true; // is null
                case UnaryPatternSyntax unary when unary.Pattern is ConstantPatternSyntax constant && constant.Expression.IsKind(SyntaxKind.NullLiteralExpression): return true; // is not null
                default:
                    return false;
            }
        }

        private static bool IsBooleanCheck(IsPatternExpressionSyntax expression)
        {
            switch (expression.Pattern)
            {
                case ConstantPatternSyntax constant when IsBoolean(constant): return true;
                case UnaryPatternSyntax unary when unary.Pattern is ConstantPatternSyntax constant && IsBoolean(constant): return true;
                default:
                    return false;
            }

            bool IsBoolean(ConstantPatternSyntax constant)
            {
                switch (constant.Expression.Kind())
                {
                    case SyntaxKind.TrueLiteralExpression:
                    case SyntaxKind.FalseLiteralExpression:
                        return true;

                    default:
                        return false;
                }
            }
        }

        private static bool CanBeSkipped(ExpressionSyntax node, SyntaxNodeAnalysisContext context)
        {
            switch (node)
            {
                case IdentifierNameSyntax _:
                    return true; // do not report checks on boolean members

                case IsPatternExpressionSyntax e:
                    if (e.Pattern is DeclarationPatternSyntax)
                    {
                        return true;  // do not report pattern checks
                    }

                    if (IsNullCheck(e))
                    {
                        return true; // do not report null pattern checks
                    }

                    if (IsBooleanCheck(e))
                    {
                        return true; // do not report boolean pattern checks
                    }

                    return false;

                case MemberAccessExpressionSyntax m:
                    return m.Name.GetTypeSymbol(context.SemanticModel)?.IsValueType is true; // do not report checks on value type members

                case BinaryExpressionSyntax b:
                {
                    if (IsNullCheck(b))
                    {
                        return true; // do not report on checks for null
                    }

                    // do not report on value types
                    return b.Right.GetTypeSymbol(context.SemanticModel)?.IsValueType is true && b.Left.GetTypeSymbol(context.SemanticModel)?.IsValueType is true;
                }

                default:
                    return false;
            }
        }

        private static void CollectLeafs(BinaryExpressionSyntax binary, Stack<ExpressionSyntax> nodes)
        {
            Collect(binary.Right.WithoutParenthesis());
            Collect(binary.Left.WithoutParenthesis());

            return;

            void Collect(ExpressionSyntax expression)
            {
                if (expression is BinaryExpressionSyntax b && b.IsAnyKind(LogicalConditions))
                {
                    CollectLeafs(b, nodes);
                }
                else
                {
                    nodes.Push(expression);
                }
            }
        }

        private static bool HasIssue(BinaryExpressionSyntax binary, SyntaxNodeAnalysisContext context)
        {
            if (binary.Parent.WithoutParenthesisParent() is BinaryExpressionSyntax parent && parent.IsAnyKind(LogicalConditions))
            {
                // ignore if we have already a parent logical expression as that will get inspected as well
                return false;
            }

            // first get all leave nodes, not the logical expressions
            var leafs = new Stack<ExpressionSyntax>();

            CollectLeafs(binary, leafs);

            // first ignore all that could be skipped, but then see if some nodes are left that could be skipped
            var hasIssues = leafs.SkipWhile(_ => CanBeSkipped(_, context))
                                 .Any(_ => CanBeSkipped(_, context));

            if (hasIssues)
            {
                // seems we have a non-number that is ordered before, so we have to report that
                return true;
            }

            // TODO RKN: it still might be that we have array access before number comparisons, so investigate that
            //  ElementAccessExpressionSyntax -> do not report on element access
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