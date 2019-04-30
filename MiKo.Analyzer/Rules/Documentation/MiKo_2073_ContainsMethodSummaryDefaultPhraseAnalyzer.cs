using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2073";

        private const string Phrase = "Determines";

        public MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.All(StartsWithPhrase)
                                                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                                                        : new[] { Issue(symbol, Phrase) };

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.Name.StartsWith("Contains", StringComparison.OrdinalIgnoreCase);

        private static bool StartsWithPhrase(string summary)
        {
            var firstWord = summary.Remove(Constants.Comments.AsynchrounouslyStartingPhrase).Trim() // skip over async starting phrase
                                   .FirstWord();

            return firstWord.Equals(Phrase, StringComparison.OrdinalIgnoreCase);
        }
    }
}