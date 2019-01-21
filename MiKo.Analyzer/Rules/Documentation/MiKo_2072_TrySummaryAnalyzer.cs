using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2072_TrySummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2072";

        private static readonly string[] Phrases = { "Try", "Tries" };

        public MiKo_2072_TrySummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Method);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(StartsWithPhrase)
                                                                                                                        ? new[] { ReportIssue(symbol, "Attempts to") }
                                                                                                                        : Enumerable.Empty<Diagnostic>();

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.Name.StartsWith("Try", StringComparison.OrdinalIgnoreCase);

        private static bool StartsWithPhrase(string summary)
        {
            // get rid of async starting phrase
            summary = summary.Remove(Constants.Comments.AsynchrounouslyStartingPhrase).Trim();

            var firstSpace = summary.IndexOf(" ", StringComparison.OrdinalIgnoreCase);
            var firstWord = firstSpace == -1 ? summary : summary.Substring(0, firstSpace);

            return firstWord.EqualsAny(Phrases);
        }
    }
}