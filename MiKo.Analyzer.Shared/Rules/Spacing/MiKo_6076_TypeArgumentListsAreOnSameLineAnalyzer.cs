using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6076_TypeArgumentListsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6076";

        public MiKo_6076_TypeArgumentListsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.TypeArgumentList);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is TypeArgumentListSyntax syntax && syntax.IsSpanningMultipleLines())
            {
                ReportDiagnostics(context, Issue(syntax));
            }
        }
    }
}