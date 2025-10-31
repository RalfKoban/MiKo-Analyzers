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
                    var operatorToken = binary.OperatorToken;
                    var expression = binary.Right;

                    if (operatorToken.IsOnSameLineAs(expression))
                    {
                        return null;
                    }

                    var spaces = expression.GetPositionWithinStartLine();

                    return Issue(operatorToken, CreateProposalForSpaces(spaces));
                }

                case PrefixUnaryExpressionSyntax unary:
                {
                    var operatorToken = unary.OperatorToken;

                    return operatorToken.IsOnSameLineAs(unary.Operand) ? null : Issue(operatorToken);
                }

                case PostfixUnaryExpressionSyntax unary:
                {
                    var operatorToken = unary.OperatorToken;
                    var expression = unary.Operand;

                    if (expression is InvocationExpressionSyntax i && i.Expression is MemberAccessExpressionSyntax maes)
                    {
                        expression = maes.Name;
                    }

                    return operatorToken.IsOnSameLineAs(expression) ? null : Issue(operatorToken);
                }

                default:
                    return null;
            }
        }
    }
}