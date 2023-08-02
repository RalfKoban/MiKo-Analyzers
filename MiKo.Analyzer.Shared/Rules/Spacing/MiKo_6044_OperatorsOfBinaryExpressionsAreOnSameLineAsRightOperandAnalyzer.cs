using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6044_OperatorsOfBinaryExpressionsAreOnSameLineAsRightOperandAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6044";

        private static readonly SyntaxKind[] BinaryExpressions =
                                                                 {
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

        public MiKo_6044_OperatorsOfBinaryExpressionsAreOnSameLineAsRightOperandAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, BinaryExpressions);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (BinaryExpressionSyntax)context.Node;

            var startLine = node.OperatorToken.GetStartingLine();
            var rightLine = node.Right.GetStartingLine();

            if (startLine != rightLine)
            {
                ReportDiagnostics(context, Issue(node.OperatorToken));
            }
        }
    }
}