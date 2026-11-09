using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6080";

        public MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.Block);

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BlockSyntax block)
            {
                var openBrace = block.OpenBraceToken;
                var closeBrace = block.CloseBraceToken;

                if (openBrace.IsOnSameLineAs(closeBrace))
                {
                    return;
                }

                var openBracePosition = openBrace.GetPositionWithinStartLine();
                var closeBracePosition = closeBrace.GetPositionWithinStartLine();

                if (openBracePosition != closeBracePosition)
                {
                    ReportDiagnostics(context, Issue(closeBrace, CreateProposalForSpaces(openBracePosition, openBracePosition - closeBracePosition)));
                }
            }
        }
    }
}