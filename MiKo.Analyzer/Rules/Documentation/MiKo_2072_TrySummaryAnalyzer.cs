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
                                                                                                                        ? new[] { Issue(symbol, "Attempts to") }
                                                                                                                        : Enumerable.Empty<Diagnostic>();

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.Name.StartsWith("Try", StringComparison.OrdinalIgnoreCase);

        private static bool StartsWithPhrase(string summary)
        {
            var firstWord = summary.Without(Constants.Comments.AsynchrounouslyStartingPhrase).Trim() // skip over async starting phrase
                                   .FirstWord();

            return firstWord.EqualsAny(Phrases);
        }
    }
}