using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2070_ReturnsSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2070";

        public MiKo_2070_ReturnsSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(StartsWithReturns)
                                                                                                                        ? new[] { ReportIssue(symbol, "Gets") }
                                                                                                                        : Enumerable.Empty<Diagnostic>();
        private static bool StartsWithReturns(string summary)
        {
            // get rid of async starting phrase
            summary = summary.Replace(Constants.Comments.AsynchrounouslyStartingPhrase, string.Empty).Trim();

            var firstSpace = summary.IndexOf(" ", StringComparison.OrdinalIgnoreCase);
            var firstWord = firstSpace == -1 ? summary : summary.Substring(0, firstSpace);

            return firstWord.EqualsAny("Return", "Returns");
        }
    }
}