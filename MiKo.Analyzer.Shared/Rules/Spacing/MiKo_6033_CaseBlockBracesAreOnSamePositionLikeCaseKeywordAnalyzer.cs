using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiKo_6033_CaseBlockBracesAreOnSamePositionLikeCaseKeywordAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6033";

        private static readonly SyntaxKind[] Sections = { SyntaxKind.SwitchSection };

        public MiKo_6033_CaseBlockBracesAreOnSamePositionLikeCaseKeywordAnalyzer() : base(Id)
        {
        }

        internal static LinePosition GetStartPosition(SwitchSectionSyntax section)
        {
            var caseKeyword = section.Labels.First().Keyword;

            return GetStartPosition(caseKeyword);
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Sections);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is SwitchSectionSyntax section && section.Statements.First() is BlockSyntax block)
            {
                var casePosition = GetStartPosition(section);

                var openBraceToken = block.OpenBraceToken;

                var openBracePosition = GetStartPosition(openBraceToken);

                if (casePosition.Line != openBracePosition.Line)
                {
                    if (casePosition.Character != openBracePosition.Character)
                    {
                        ReportDiagnostics(context, Issue(openBraceToken));
                    }
                }
            }
        }
    }
}