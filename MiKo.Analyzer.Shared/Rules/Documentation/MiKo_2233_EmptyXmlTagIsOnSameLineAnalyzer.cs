using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2233_EmptyXmlTagIsOnSameLineAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2233";

        public MiKo_2233_EmptyXmlTagIsOnSameLineAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeXmlEmptyElement, SyntaxKind.XmlEmptyElement);

        private void AnalyzeXmlEmptyElement(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            if (node.IsSpanningMultipleLines())
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}