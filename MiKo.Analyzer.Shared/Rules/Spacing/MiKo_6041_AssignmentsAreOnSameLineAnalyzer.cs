using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6041_AssignmentsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6041";

        private static readonly SyntaxKind[] SimpleAssignmentExpressions =
                                                                           {
                                                                               // AssignmentExpressionSyntax
                                                                               SyntaxKind.AddAssignmentExpression,
                                                                               SyntaxKind.SubtractAssignmentExpression,
                                                                               SyntaxKind.MultiplyAssignmentExpression,
                                                                               SyntaxKind.DivideAssignmentExpression,
                                                                               SyntaxKind.ModuloAssignmentExpression,
                                                                               SyntaxKind.AndAssignmentExpression,
                                                                               SyntaxKind.ExclusiveOrAssignmentExpression,
                                                                               SyntaxKind.OrAssignmentExpression,
                                                                               SyntaxKind.LeftShiftAssignmentExpression,
                                                                               SyntaxKind.RightShiftAssignmentExpression,
                                                                               SyntaxKind.CoalesceAssignmentExpression,
                                                                               SyntaxKind.SimpleAssignmentExpression,

                                                                               // EqualsValueClauseSyntax
                                                                               SyntaxKind.EqualsValueClause,
                                                                           };

        public MiKo_6041_AssignmentsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SimpleAssignmentExpressions);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case AssignmentExpressionSyntax assignment:
                    AnalyzeAssignmentExpression(context, assignment);

                    break;

                case EqualsValueClauseSyntax clause:
                    AnalyzeEqualsValueClause(context, clause);

                    break;
            }
        }

        private void AnalyzeAssignmentExpression(SyntaxNodeAnalysisContext context, AssignmentExpressionSyntax node)
        {
            var operatorToken = node.OperatorToken;

            var startLine = operatorToken.GetStartingLine();
            var leftLine = node.Left.GetStartingLine();
            var rightLine = node.Right.GetStartingLine();

            if (startLine != leftLine || startLine != rightLine)
            {
                ReportIssue(context, operatorToken);
            }
        }

        private void AnalyzeEqualsValueClause(SyntaxNodeAnalysisContext context, EqualsValueClauseSyntax node)
        {
            var startLine = node.EqualsToken.GetStartingLine();
            var expressionLine = node.Value.GetStartingLine();

            if (startLine != expressionLine)
            {
                ReportIssue(context, node);
            }
            else
            {
                var sibling = node.PreviousSiblingNodeOrToken();
                var siblingStartLine = sibling.GetStartingLine();

                if (startLine != siblingStartLine)
                {
                    ReportIssue(context, node);
                }
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, EqualsValueClauseSyntax node)
        {
            switch (node.Value)
            {
                // arrays and collections spanning multiple lines are allowed
                case InitializerExpressionSyntax initializer when initializer.OpenBraceToken.GetStartingLine() != initializer.CloseBraceToken.GetStartingLine():
                    break;
#if  VS2022
                case CollectionExpressionSyntax expression when expression.OpenBracketToken.GetStartingLine() != expression.CloseBracketToken.GetStartingLine():
                    break;
#endif

                default:
                    ReportIssue(context, node.EqualsToken);

                    break;
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxToken token) => ReportDiagnostics(context, Issue(token));
    }
}