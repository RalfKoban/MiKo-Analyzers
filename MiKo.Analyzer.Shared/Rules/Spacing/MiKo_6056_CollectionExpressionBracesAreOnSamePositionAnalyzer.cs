#if VS2022

using System.Linq;

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
                    return argumentList.OpenParenToken.GetEndPosition();

                case EqualsValueClauseSyntax clause:
                    return clause.EqualsToken.GetPositionAfterEnd();

                default:
                    return syntax.CloseBracketToken.GetStartPosition();
            }
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
            if (node is CollectionExpressionSyntax syntax && syntax.Elements.None(SyntaxKind.SpreadElement))
            {
                var openBracketToken = syntax.OpenBracketToken;
                var openBracketPosition = openBracketToken.GetStartPosition();

                var startPosition = GetStartPosition(syntax);

                if (openBracketPosition.Line != startPosition.Line && openBracketPosition.Character != startPosition.Character)
                {
                    return Issue(openBracketToken, CreateProposalForSpaces(startPosition.Character));
                }

                var closeBracketToken = syntax.CloseBracketToken;
                var closeBracketPosition = closeBracketToken.GetStartPosition();

                if (openBracketPosition.Line != closeBracketPosition.Line && openBracketPosition.Character != closeBracketPosition.Character)
                {
                    return Issue(closeBracketToken, CreateProposalForSpaces(openBracketPosition.Character));
                }
            }

            return null;
        }
    }
}

#endif