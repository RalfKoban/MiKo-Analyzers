using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2016_AsyncMethodDefaultPhraseAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2016";

        private const string Phrase = Constants.Comments.AsynchronouslyStartingPhrase;

        public MiKo_2016_AsyncMethodDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IMethodSymbol method && method.IsAsyncTaskBased();

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, Phrase);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = string.Empty;
            comparison = StringComparison.Ordinal;

            if (valueText.AsSpan().TrimStart().StartsWith(Phrase))
            {
                return false;
            }

            problematicText = valueText.FirstWord();

            return true;
        }
    }
}