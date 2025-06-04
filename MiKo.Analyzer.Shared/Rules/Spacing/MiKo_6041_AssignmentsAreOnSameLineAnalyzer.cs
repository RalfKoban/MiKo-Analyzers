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

        private static bool IgnoreIssue(SyntaxNode node)
        {
            switch (node)
            {
                // arrays and collections spanning multiple lines are allowed
                case InitializerExpressionSyntax initializer when initializer.OpenBraceToken.IsOnSameLineAs(initializer.CloseBraceToken) is false:
                    return true;
#if  VS2022
                case CollectionExpressionSyntax expression when expression.OpenBracketToken.IsOnSameLineAs(expression.CloseBracketToken) is false:
                    return true;
#endif

                default:
                    return false;
            }
        }

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

        private void AnalyzeAssignmentExpression(in SyntaxNodeAnalysisContext context, AssignmentExpressionSyntax node)
        {
            var operatorToken = node.OperatorToken;
            var allOnSameLine = operatorToken.IsOnSameLineAs(node.Left) && operatorToken.IsOnSameLineAs(node.Right);

            if (allOnSameLine is false)
            {
                ReportIssue(context, operatorToken);
            }
        }

        private void AnalyzeEqualsValueClause(in SyntaxNodeAnalysisContext context, EqualsValueClauseSyntax node)
        {
            var equalsToken = node.EqualsToken;
            var allOnSameLine = equalsToken.IsOnSameLineAs(node.Value) && equalsToken.IsOnSameLineAs(node.PreviousSiblingNodeOrToken());

            if (allOnSameLine is false)
            {
                ReportIssue(context, node);
            }
        }

        private void ReportIssue(in SyntaxNodeAnalysisContext context, EqualsValueClauseSyntax node)
        {
            if (IgnoreIssue(node.Value) is false)
            {
                ReportIssue(context, node.EqualsToken);
            }
        }

        private void ReportIssue(in SyntaxNodeAnalysisContext context, in SyntaxToken token)
        {
            if (IgnoreIssue(token.Parent?.Parent) is false)
            {
                ReportDiagnostics(context, Issue(token));
            }
        }
    }
}