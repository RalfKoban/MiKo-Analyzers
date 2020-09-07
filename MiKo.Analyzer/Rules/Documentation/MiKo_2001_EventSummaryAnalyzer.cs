using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2001_EventSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2001";

        internal const string Phrase = "Occurs ";

        public MiKo_2001_EventSummaryAnalyzer() : base(Id, SymbolKind.Event)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(_ => _.TrimStart().StartsWith(Phrase, StringComparison.Ordinal))
                                                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                                                        : new[] { Issue(symbol, Phrase) };
    }
}