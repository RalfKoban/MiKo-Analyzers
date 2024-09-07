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
                                                           };

        public MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Expressions);

        private static bool IsOnSingleLineLocal(SyntaxNode s)
        {
            var span = s.GetLocation().GetLineSpan();

            return span.StartLinePosition.Line == span.EndLinePosition.Line;
        }

        private static bool IsOnSingleLine(ExpressionSyntax syntax)
        {
            switch (syntax)
            {
                case ParenthesizedExpressionSyntax parenthesized:
                    return IsOnSingleLine(parenthesized);

                case BinaryExpressionSyntax binary:
                    return IsOnSingleLine(binary);

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
            return IsOnSingleLine(leftCondition) && IsOnSingleLine(rightCondition);
        }

        private static bool IsOnSingleLine(ParenthesizedExpressionSyntax parenthesized)
        {
            if (IsOnSingleLine(parenthesized.Expression))
            {
                // we have a parenthesized one, so let's check also the parenthesis
                var openParenthesisSpan = parenthesized.OpenParenToken.GetLocation().GetLineSpan();
                var closeParenthesisSpan = parenthesized.CloseParenToken.GetLocation().GetLineSpan();

                if (openParenthesisSpan.StartLinePosition.Line == closeParenthesisSpan.EndLinePosition.Line)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ShallAnalyzeNode(BinaryExpressionSyntax syntax)
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
            if (context.Node is BinaryExpressionSyntax syntax && ShallAnalyzeNode(syntax))
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