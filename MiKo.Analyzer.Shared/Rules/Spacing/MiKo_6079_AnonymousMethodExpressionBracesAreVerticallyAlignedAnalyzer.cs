using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6079_AnonymousMethodExpressionBracesAreVerticallyAlignedAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6079";

        public MiKo_6079_AnonymousMethodExpressionBracesAreVerticallyAlignedAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.AnonymousMethodExpression);

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is AnonymousMethodExpressionSyntax node && node.Block is BlockSyntax block)
            {
                var keyword = node.DelegateKeyword;

                var openBrace = block.OpenBraceToken;

                if (keyword.IsOnSameLineAs(openBrace) is false)
                {
                    var keywordPosition = keyword.GetPositionWithinStartLine();
                    var openBracePosition = openBrace.GetPositionWithinStartLine();

                    if (keywordPosition != openBracePosition)
                    {
                        var issue = Issue(openBrace, CreateProposalForSpaces(keywordPosition));

                        ReportDiagnostics(context, issue);
                    }
                }
            }
        }
    }
}