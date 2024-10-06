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
                        return ifStatement.IfKeyword.GetEndPosition().Character - 1;

                    case ReturnStatementSyntax returnStatement:
                        return returnStatement.ReturnKeyword.GetEndPosition().Character - 2;

                    case WhenClauseSyntax whenClause:
                        return whenClause.WhenKeyword.GetEndPosition().Character - 2;
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