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

        private static readonly SyntaxKind[] Expressions =
                                                           {
                                                               // binary ones
                                                               SyntaxKind.LogicalOrExpression,
                                                               SyntaxKind.LogicalAndExpression,
                                                               SyntaxKind.BitwiseOrExpression,
                                                               SyntaxKind.BitwiseAndExpression,
                                                               SyntaxKind.ExclusiveOrExpression,

                                                               // prefix unary ones
                                                               SyntaxKind.UnaryPlusExpression,
                                                               SyntaxKind.UnaryMinusExpression,
                                                               SyntaxKind.BitwiseNotExpression,
                                                               SyntaxKind.LogicalNotExpression,
                                                               SyntaxKind.PreIncrementExpression,
                                                               SyntaxKind.PreDecrementExpression,
                                                               SyntaxKind.AddressOfExpression,
                                                               SyntaxKind.PointerIndirectionExpression,
                                                               SyntaxKind.IndexExpression,

                                                               // postfix unary ones
                                                               SyntaxKind.PostIncrementExpression,
                                                               SyntaxKind.PostDecrementExpression,
                                                               SyntaxKind.SuppressNullableWarningExpression,
                                                           };

        public MiKo_6044_BooleanOperatorsAreOnSameLineAsRightOperandAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Expressions);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var issue = AnalyzeNode(context.Node);

            if (issue != null)
            {
                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic AnalyzeNode(SyntaxNode node)
        {
            switch (node)
            {
                case BinaryExpressionSyntax binary:
                {
                    var startLine = binary.OperatorToken.GetStartingLine();
                    var rightPosition = binary.Right.GetStartPosition();

                    return startLine != rightPosition.Line
                           ? Issue(binary.OperatorToken, CreateProposalForSpaces(rightPosition.Character))
                           : null;
                }

                case PrefixUnaryExpressionSyntax unary:
                {
                    var startLine = unary.Operand.GetStartingLine();
                    var operatorLine = unary.OperatorToken.GetStartingLine();

                    return startLine != operatorLine
                           ? Issue(unary.OperatorToken)
                           : null;
                }

                case PostfixUnaryExpressionSyntax unary:
                {
                    var startLine = unary.Operand.GetStartingLine();
                    var operatorLine = unary.OperatorToken.GetStartingLine();

                    return startLine != operatorLine
                           ? Issue(unary.OperatorToken)
                           : null;
                }

                default:
                    return null;
            }
        }
    }
}