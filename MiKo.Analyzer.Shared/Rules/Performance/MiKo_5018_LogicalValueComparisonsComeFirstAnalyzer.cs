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

        private static bool IsElementAccess(ExpressionSyntax expression) => expression is BinaryExpressionSyntax binary && IsElementAccess(binary);

        private static bool IsElementAccess(BinaryExpressionSyntax expression) => expression.Left.IsKind(SyntaxKind.ElementAccessExpression) || expression.Right.IsKind(SyntaxKind.ElementAccessExpression);

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

        private static bool IsValueType(ExpressionSyntax node, SyntaxNodeAnalysisContext context)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                    return true;

                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
#if VS2022
                case SyntaxKind.Utf8StringLiteralExpression:
#endif
                    return false;

                default:
                    return node.GetTypeSymbol(context.SemanticModel)?.IsValueType is true;
            }
        }

        private static bool CanBeSkipped(ExpressionSyntax node, SyntaxNodeAnalysisContext context)
        {
            switch (node)
            {
                case IdentifierNameSyntax _:
                    return true; // do not report checks on boolean members

                case PrefixUnaryExpressionSyntax pu when pu.IsKind(SyntaxKind.LogicalNotExpression):
                    return true; // do not report on boolean !xyz checks

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
                    return IsValueType(m.Name, context); // do not report checks on value type members

                case BinaryExpressionSyntax b:
                {
                    if (IsNullCheck(b))
                    {
                        return true; // do not report on checks for null
                    }

                    // do not report on value types
                    return IsValueType(b.Right, context) && IsValueType(b.Left, context);
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
            if (leafs.SkipWhile(_ => CanBeSkipped(_, context)).Any(_ => CanBeSkipped(_, context)))
            {
                // seems we have a non-number that is ordered before, so we have to report that
                return true;
            }

            // it still might be that we have array access before number comparisons, so investigate that
            // (note: as the element access nodes can be in between others, we have to sort them out
            return leafs.SkipWhile(_ => IsElementAccess(_) is false) // jump over all non-element access leafs
                        .SkipWhile(IsElementAccess) // now jump over all element access leafs
                        .Any(_ => CanBeSkipped(_, context)); // now no leaf should be left, otherwise we have an issue
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