using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6036_OpenBracesAreOnSamePositionLikeArrowOfLambdaAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6036";

        public MiKo_6036_OpenBracesAreOnSamePositionLikeArrowOfLambdaAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ParenthesizedLambdaExpression, SyntaxKind.SimpleLambdaExpression);

        private static LinePosition GetStartPosition(LambdaExpressionSyntax lambda)
        {
            var arrowToken = lambda.ArrowToken;

            var position = arrowToken.GetStartPosition();

            return new LinePosition(position.Line, position.Character + Constants.Indentation - arrowToken.Span.Length);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var lambda = (LambdaExpressionSyntax)context.Node;
            var block = lambda.Block;

            if (block != null)
            {
                var operatorPosition = GetStartPosition(lambda);

                var openBraceToken = block.OpenBraceToken;
                var openBracePosition = openBraceToken.GetStartPosition();

                if (operatorPosition.Line != openBracePosition.Line && operatorPosition.Character != openBracePosition.Character)
                {
                    var issue = Issue(openBraceToken, CreateProposalForSpaces(operatorPosition.Character));

                    ReportDiagnostics(context, issue);
                }
            }
        }
    }
}