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

        internal const string StartingPhrase = "Attempts to";

        internal static readonly string[] Words = { "Try", "Tries" };

        public MiKo_2072_TrySummaryAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries) => summaries.Any(StartsWithPhrase)
                                                                                                                                                 ? new[] { Issue(symbol, StartingPhrase) }
                                                                                                                                                 : Enumerable.Empty<Diagnostic>();

        private static bool StartsWithPhrase(string summary)
        {
            var firstWord = summary.Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                                   .FirstWord();

            return firstWord.EqualsAny(Words);
        }
    }
}