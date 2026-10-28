using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    /// <inheritdoc/>
    /// <seealso cref="MiKo_6032_MultilineParametersAreIndentedToRightAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6077_SingleParameterListsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6077";

        public MiKo_6077_SingleParameterListsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ParameterList);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ParameterListSyntax syntax && syntax.IsSpanningMultipleLines() && syntax.Parameters.Count <= 1)
            {
                ReportDiagnostics(context, Issue(syntax));
            }
        }
    }
}