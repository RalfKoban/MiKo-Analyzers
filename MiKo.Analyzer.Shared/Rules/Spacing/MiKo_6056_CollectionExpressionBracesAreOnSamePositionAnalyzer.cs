#if VS2022 || VS2026

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6056_CollectionExpressionBracesAreOnSamePositionAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6056";

        public MiKo_6056_CollectionExpressionBracesAreOnSamePositionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.CollectionExpression);

        private static LinePosition GetStartPosition(CollectionExpressionSyntax syntax)
        {
            switch (syntax.Parent)
            {
                case ArgumentSyntax argument when argument.Parent is ArgumentListSyntax argumentList:
                {
                    var firstArgument = argumentList.Arguments[0];

                    if (firstArgument == argument)
                    {
                        return argumentList.OpenParenToken.GetEndPosition();
                    }

                    return firstArgument.GetStartPosition();
                }

                case EqualsValueClauseSyntax clause:
                    return clause.EqualsToken.GetPositionAfterEnd();

                case AssignmentExpressionSyntax assignment:
                    return assignment.OperatorToken.GetPositionAfterEnd();

                case ArrowExpressionClauseSyntax clause:
                    return clause.ArrowToken.GetPositionAfterEnd();

                case ReturnStatementSyntax statement:
                    return statement.ReturnKeyword.GetPositionAfterEnd();

                default:
                    return syntax.OpenBracketToken.GetStartPosition();
            }
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is CollectionExpressionSyntax node)
            {
                var issue = AnalyzeNode(node);

                if (issue != null)
                {
                    ReportDiagnostics(context, issue);
                }
            }
        }

        private Diagnostic AnalyzeNode(CollectionExpressionSyntax syntax)
        {
            var openBracket = syntax.OpenBracketToken;
            var closeBracket = syntax.CloseBracketToken;

            if (openBracket.IsOnSameLineAs(closeBracket))
            {
                return null;
            }

            var startPosition = GetStartPosition(syntax);
            var openBracketPosition = openBracket.GetStartPosition();

            if (NotVerticallyAligned(startPosition, openBracketPosition))
            {
                return Issue(openBracket, CreateProposalForSpaces(startPosition.Character));
            }

            if (NotVerticallyAligned(closeBracket, openBracketPosition))
            {
                return Issue(closeBracket, CreateProposalForSpaces(openBracketPosition.Character));
            }

            return null;
        }
    }
}

#endif