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

        internal const string StartingPhrase = Constants.Comments.DeterminesWhetherPhrase;

        public MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Name.StartsWith("Contains", StringComparison.OrdinalIgnoreCase) && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries) => summaries.All(StartsWithPhrase)
                                                                                                                                                 ? Enumerable.Empty<Diagnostic>()
                                                                                                                                                 : new[] { Issue(symbol, StartingPhrase) };

        private static bool StartsWithPhrase(string summary)
        {
            // skip over async starting phrase
            var withoutAsync = summary.Without(Constants.Comments.AsynchrounouslyStartingPhrase);

            var firstWord = withoutAsync.FirstWord();

            if (firstWord.Equals(StartingPhrase.FirstWord(), StringComparison.OrdinalIgnoreCase))
            {
                var secondWord = withoutAsync.SecondWord();

                return secondWord.Equals(StartingPhrase.SecondWord(), StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}