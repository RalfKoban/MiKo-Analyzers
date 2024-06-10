using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6033_CaseBlockBracesAreOnSamePositionLikeCaseKeywordAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6033";

        private static readonly SyntaxKind[] Sections = { SyntaxKind.SwitchSection };

        public MiKo_6033_CaseBlockBracesAreOnSamePositionLikeCaseKeywordAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Sections);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is SwitchSectionSyntax section && section.Statements.First() is BlockSyntax block)
            {
                var caseToken = section.Labels.First().Keyword;
                var openBraceToken = block.OpenBraceToken;

                var casePosition = caseToken.GetStartPosition();
                var openBracePosition = openBraceToken.GetStartPosition();

                if (casePosition.Line != openBracePosition.Line && casePosition.Character != openBracePosition.Character)
                {
                    var issue = Issue(openBraceToken, CreateProposalForSpaces(casePosition.Character));

                    ReportDiagnostics(context, issue);
                }
            }
        }
    }
}