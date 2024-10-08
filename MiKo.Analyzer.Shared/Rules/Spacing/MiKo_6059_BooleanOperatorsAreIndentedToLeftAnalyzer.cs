using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6059";

        private static readonly SyntaxKind[] Expressions =
                                                           {
                                                               // binary ones
                                                               SyntaxKind.LogicalOrExpression,
                                                               SyntaxKind.LogicalAndExpression,
                                                               SyntaxKind.BitwiseOrExpression,
                                                               SyntaxKind.BitwiseAndExpression,
                                                               SyntaxKind.ExclusiveOrExpression,
                                                           };

        public MiKo_6059_BooleanOperatorsAreIndentedToLeftAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Expressions);

        private static SyntaxToken GetOutdentedOrientationToken(BinaryExpressionSyntax binary)
        {
            foreach (var item in binary.Ancestors())
            {
                switch (item)
                {
                    case IfStatementSyntax ifStatement:
                        return ifStatement.IfKeyword;

                    case ReturnStatementSyntax returnStatement:
                        return returnStatement.ReturnKeyword;

                    case WhenClauseSyntax whenClause:
                        return whenClause.WhenKeyword;

                    case ArgumentListSyntax argument:
                        return argument.OpenParenToken;

                    case EqualsValueClauseSyntax clause:
                        return clause.EqualsToken;

                    case AssignmentExpressionSyntax assignment:
                        return assignment.OperatorToken;
                }
            }

            return binary.OperatorToken;
        }

        private static int GetOutdentedPosition(BinaryExpressionSyntax binary)
        {
            foreach (var item in binary.Ancestors())
            {
                switch (item)
                {
                    case IfStatementSyntax ifStatement:
                        return ifStatement.IfKeyword.GetPositionWithinEndLine() - 1;

                    case ReturnStatementSyntax returnStatement:
                        return returnStatement.ReturnKeyword.GetPositionWithinEndLine() - 2;

                    case WhenClauseSyntax whenClause:
                        return whenClause.WhenKeyword.GetPositionWithinEndLine() - 2;

                    case ArgumentListSyntax argument:
                        return argument.OpenParenToken.GetPositionWithinEndLine() - 3;

                    case EqualsValueClauseSyntax clause:
                        return clause.EqualsToken.GetPositionWithinEndLine() - 2;

                    case AssignmentExpressionSyntax assignment:
                        return assignment.OperatorToken.GetPositionWithinEndLine() - 2;
                }
            }

            return binary.OperatorToken.GetPositionWithinStartLine();
        }

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
            if (node is BinaryExpressionSyntax binary)
            {
                var operatorToken = binary.OperatorToken;

                if (binary.Left.GetEndingLine() < operatorToken.GetStartingLine())
                {
                    var orientationToken = GetOutdentedOrientationToken(binary);

                    var orientationStartingLine = orientationToken.GetStartingLine();
                    var operatorTokenStartingLine = operatorToken.GetStartingLine();

                    if (orientationStartingLine != operatorTokenStartingLine)
                    {
                        var spaces = GetOutdentedPosition(binary);

                        if (operatorToken.GetPositionWithinStartLine() != spaces)
                        {
                            return Issue(operatorToken, CreateProposalForSpaces(spaces));
                        }
                    }
                }
            }

            return null;
        }
    }
}