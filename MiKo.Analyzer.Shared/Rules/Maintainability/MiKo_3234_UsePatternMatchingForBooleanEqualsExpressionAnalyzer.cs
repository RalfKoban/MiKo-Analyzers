using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3234_UsePatternMatchingForBooleanEqualsExpressionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3234";

        public MiKo_3234_UsePatternMatchingForBooleanEqualsExpressionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeEqualsExpression, SyntaxKind.EqualsExpression);
            context.RegisterSyntaxNodeAction(AnalyzePatternExpression, SyntaxKind.IsPatternExpression);
        }

        private static bool HasIssue(BinaryExpressionSyntax node)
        {
            var left = node.Left;
            var right = node.Right;

            if (right.IsKind(SyntaxKind.TrueLiteralExpression))
            {
                return HasIssueCore(left);
            }

            if (left.IsKind(SyntaxKind.TrueLiteralExpression))
            {
                return HasIssueCore(right);
            }

            return false;
        }

        private static bool HasIssue(IsPatternExpressionSyntax node)
        {
            if (node.Pattern is ConstantPatternSyntax c && c.Expression.IsKind(SyntaxKind.TrueLiteralExpression))
            {
                return HasIssueCore(node.Expression);
            }

            return false;
        }

        private static bool HasIssueCore(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case InvocationExpressionSyntax i when IsEqualsCall(i):
                case ConditionalAccessExpressionSyntax c when IsEqualsCall(c):
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsEqualsCall(ConditionalAccessExpressionSyntax expression)
        {
            var node = expression;

            while (node != null)
            {
                switch (node.WhenNotNull)
                {
                    case ConditionalAccessExpressionSyntax c:
                    {
                        node = c;

                        continue;
                    }

                    case InvocationExpressionSyntax i:
                        return IsEqualsCall(i);
                }

                break;
            }

            return false;
        }

        private static bool IsEqualsCall(InvocationExpressionSyntax expression)
        {
            var name = expression.GetName();

            if (name is nameof(Equals))
            {
                var arguments = expression.ArgumentList.Arguments;

                if (arguments.Count is 1)
                {
                    var syntax = arguments[0].Expression;

                    switch (syntax.Kind())
                    {
                        case SyntaxKind.TrueLiteralExpression:
                        case SyntaxKind.FalseLiteralExpression:
                        case SyntaxKind.NumericLiteralExpression:
                        case SyntaxKind.NullLiteralExpression:
                        case SyntaxKind.CharacterLiteralExpression:
                        case SyntaxKind.StringLiteralExpression:
                        // Attention! We cannot use pattern matching for UTF 8 strings as they are no constant values and can never be null!
                        // case SyntaxKind.Utf8StringLiteralExpression:
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax node && HasIssue(node))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }

        private void AnalyzePatternExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is IsPatternExpressionSyntax node && HasIssue(node))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}