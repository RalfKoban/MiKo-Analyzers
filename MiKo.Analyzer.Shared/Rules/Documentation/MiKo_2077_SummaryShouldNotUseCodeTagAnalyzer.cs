using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2077_SummaryShouldNotUseCodeTagAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2077";

        public MiKo_2077_SummaryShouldNotUseCodeTagAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event);

        // TODO RKN: Consolidate this into SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            foreach (var summaryXml in summaryXmls)
            {
                foreach (var code in summaryXml.GetXmlSyntax(Constants.XmlTag.Code))
                {
                    // we have an issue
                    yield return Issue(symbol.Name, code);
                }
            }
        }
    }
}