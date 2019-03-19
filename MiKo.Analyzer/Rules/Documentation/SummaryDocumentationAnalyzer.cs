using System;
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

        protected static IEnumerable<string> GetSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Summary));

        protected static IEnumerable<string> GetOverloadSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Overloads, Constants.XmlTag.Summary));

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => AnalyzeSummaries(symbol, commentXml);

        protected IEnumerable<Diagnostic> AnalyzeSummaries(ISymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            var summaries = GetSummaries(commentXml);

            return summaries.Any()
                       ? AnalyzeSummary(symbol, summaries)
                       : Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => Enumerable.Empty<Diagnostic>();
    }
}