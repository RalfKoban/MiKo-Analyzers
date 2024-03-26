using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6044_BooleanOperatorsAreOnSameLineAsRightOperandAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6044";

        private static readonly SyntaxKind[] BinaryExpressions =
                                                                 {
                                                                     SyntaxKind.LogicalOrExpression,
                                                                     SyntaxKind.LogicalAndExpression,
                                                                     SyntaxKind.BitwiseOrExpression,
                                                                     SyntaxKind.BitwiseAndExpression,
                                                                     SyntaxKind.ExclusiveOrExpression,
                                                                 };

        public MiKo_6044_BooleanOperatorsAreOnSameLineAsRightOperandAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, BinaryExpressions);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (BinaryExpressionSyntax)context.Node;

            var startLine = node.OperatorToken.GetStartingLine();
            var rightPosition = node.Right.GetStartPosition();

            if (startLine != rightPosition.Line)
            {
                var issue = Issue(node.OperatorToken, CreateProposalForSpaces(rightPosition.Character));

                ReportDiagnostics(context, issue);
            }
        }
    }
}