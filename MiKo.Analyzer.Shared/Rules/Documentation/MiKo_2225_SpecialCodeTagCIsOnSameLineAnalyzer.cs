using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2225_SpecialCodeTagCIsOnSameLineAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2225";

        public MiKo_2225_SpecialCodeTagCIsOnSameLineAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeXmlElement, SyntaxKind.XmlElement);

        private void AnalyzeXmlElement(SyntaxNodeAnalysisContext context)
        {
            var node = (XmlElementSyntax)context.Node;

            if (node.GetXmlTagName() == Constants.XmlTag.C)
            {
                var start = node.StartTag.GetStartingLine();
                var end = node.EndTag.GetStartingLine();

                if (start != end)
                {
                    ReportDiagnostics(context, Issue(node));
                }
            }
        }
    }
}