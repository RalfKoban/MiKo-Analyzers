using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2041_InvalidXmlInSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2041";

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2041_InvalidXmlInSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            List<Diagnostic> findings = null;

            foreach (var phrase in summaries.SelectMany(_ => Constants.Comments.InvalidSummaryCrefPhrases.Where(__ => _.Contains(__, Comparison))))
            {
                if (findings == null) findings = new List<Diagnostic>();
                findings.Add(Issue(symbol, phrase + Constants.Comments.XmlElementEndingTag));
            }

            return findings ?? Enumerable.Empty<Diagnostic>();
        }
    }
}