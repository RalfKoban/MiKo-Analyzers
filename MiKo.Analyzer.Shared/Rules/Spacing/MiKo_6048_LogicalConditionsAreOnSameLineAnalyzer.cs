using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6048";

        private static readonly SyntaxKind[] Expressions =
                                                           {
                                                               SyntaxKind.LogicalAndExpression,
                                                               SyntaxKind.LogicalOrExpression,
                                                               SyntaxKind.IsPatternExpression,
                                                               SyntaxKind.ParenthesizedExpression,
                                                               SyntaxKind.AddExpression,
                                                               SyntaxKind.SubtractExpression,
                                                               SyntaxKind.MultiplyExpression,
                                                               SyntaxKind.DivideExpression,
                                                               SyntaxKind.ModuloExpression,
                                                               SyntaxKind.LeftShiftExpression,
                                                               SyntaxKind.RightShiftExpression,
                                                               SyntaxKind.LogicalOrExpression,
                                                               SyntaxKind.LogicalAndExpression,
                                                               SyntaxKind.BitwiseOrExpression,
                                                               SyntaxKind.BitwiseAndExpression,
                                                               SyntaxKind.ExclusiveOrExpression,
                                                               SyntaxKind.EqualsExpression,
                                                               SyntaxKind.NotEqualsExpression,
                                                               SyntaxKind.LessThanExpression,
                                                               SyntaxKind.LessThanOrEqualExpression,
                                                               SyntaxKind.GreaterThanExpression,
                                                               SyntaxKind.GreaterThanOrEqualExpression,
                                                               SyntaxKind.IsExpression,
                                                               SyntaxKind.AsExpression,
                                                               SyntaxKind.CoalesceExpression,
                                                           };

        public MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Expressions);

        private static bool IsOnSingleLineLocal(SyntaxNode s)
        {
            var span = s.GetLineSpan();

            return span.StartLinePosition.Line == span.EndLinePosition.Line;
        }

        private static bool IsOnSingleLine(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case ParenthesizedExpressionSyntax parenthesized:
                    return IsOnSingleLine(parenthesized);

                case BinaryExpressionSyntax binary:
                    return IsOnSingleLine(binary);

                case IsPatternExpressionSyntax isPattern:
                    return IsOnSingleLine(isPattern);

                default:
                    return IsOnSingleLineLocal(syntax);
            }
        }

        private static bool IsOnSingleLine(BinaryExpressionSyntax binary)
        {
            if (IsOnSingleLineLocal(binary))
            {
                return true;
            }

            var leftCondition = binary.Left;
            var rightCondition = binary.Right;

            var leftSpan = leftCondition.GetLineSpan();
            var rightSpan = rightCondition.GetLineSpan();

            // let's see if both conditions are on same line
            if (leftSpan.EndLinePosition.Line == rightSpan.StartLinePosition.Line)
            {
                if (leftSpan.StartLinePosition.Line == rightSpan.EndLinePosition.Line)
                {
                    // both are on same line
                    return true;
                }

                // at least one condition spans multiple lines
                return false;
            }

            // they span different lines
            return false;
        }

        private static bool IsOnSingleLine(ParenthesizedExpressionSyntax parenthesized)
        {
            if (IsOnSingleLine(parenthesized.Expression))
            {
                // we have a parenthesized one, so let's check also the parenthesis
                var openParenthesisSpan = parenthesized.OpenParenToken.GetLineSpan();
                var closeParenthesisSpan = parenthesized.CloseParenToken.GetLineSpan();

                if (openParenthesisSpan.StartLinePosition.Line == closeParenthesisSpan.EndLinePosition.Line)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsOnSingleLine(IsPatternExpressionSyntax isPattern)
        {
            if (IsOnSingleLineLocal(isPattern))
            {
                return true;
            }

            var leftCondition = isPattern.Expression;
            var rightCondition = isPattern.Pattern;

            var leftSpan = leftCondition.GetLocation().GetLineSpan();
            var rightSpan = rightCondition.GetLocation().GetLineSpan();

            // let's see if both conditions are on same line
            if (leftSpan.EndLinePosition.Line == rightSpan.StartLinePosition.Line)
            {
                if (leftSpan.StartLinePosition.Line == rightSpan.EndLinePosition.Line)
                {
                    // both are on same line
                    return true;
                }

                // at least one condition spans multiple lines
                return false;
            }

            // they span different lines
            return false;
        }

        private static bool ShallAnalyzeNode(ExpressionSyntax syntax)
        {
            switch (syntax.Parent)
            {
                case BinaryExpressionSyntax parent when parent.IsAnyKind(Expressions):
                case ParenthesizedExpressionSyntax parenthesized when parenthesized.Parent.IsAnyKind(Expressions):
                    return false; // ignore as the parent gets investigated already

                default:
                    return true;
            }
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ExpressionSyntax syntax && ShallAnalyzeNode(syntax))
            {
                if (IsOnSingleLine(syntax))
                {
                    // nothing to report
                    return;
                }

                ReportDiagnostics(context, Issue(syntax));
            }
        }
    }
}