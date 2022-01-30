using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SummaryDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected SummaryDocumentationAnalyzer(string diagnosticId, SymbolKind symbolKind) : base(diagnosticId, symbolKind)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml) => AnalyzeSummaries(symbol, commentXml);

        protected IEnumerable<Diagnostic> AnalyzeSummaries(ISymbol symbol, string commentXml)
        {
            var summaries = CommentExtensions.GetSummaries(commentXml);

            return summaries.Any()
                       ? AnalyzeSummary(symbol, summaries)
                       : Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => Enumerable.Empty<Diagnostic>();
    }
}