using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6047_SwitchExpressionBracesAreOnSamePositionLikeSwitchKeywordAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6047";

        private static readonly SyntaxKind[] Expressions = { SyntaxKind.SwitchExpression };

        public MiKo_6047_SwitchExpressionBracesAreOnSamePositionLikeSwitchKeywordAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Expressions);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is SwitchExpressionSyntax syntax)
            {
                var switchPosition = syntax.SwitchKeyword.GetPositionAfterEnd();

                var openBraceToken = syntax.OpenBraceToken;

                if (NotVerticallyAligned(openBraceToken, switchPosition))
                {
                    ReportDiagnostics(context, Issue(openBraceToken, CreateProposalForSpaces(switchPosition.Character)));
                }
            }
        }
    }
}