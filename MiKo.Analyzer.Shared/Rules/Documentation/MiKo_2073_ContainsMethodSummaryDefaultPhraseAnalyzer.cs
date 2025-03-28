using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2073";

        private const string StartingPhrase = Constants.Comments.DeterminesWhetherPhrase;

        private static readonly string StartingPhraseFirstWord = StartingPhrase.FirstWord();

        private static readonly string StartingPhraseSecondWord = StartingPhrase.SecondWord();

        public MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IMethodSymbol method && method.Name.StartsWith("Contains", StringComparison.OrdinalIgnoreCase);

        protected override Diagnostic TextStartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, StartingPhrase);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = StringComparison.OrdinalIgnoreCase;

            var withoutAsync = valueText.Without(Constants.Comments.AsynchronouslyStartingPhrase); // skip over async starting phrase

            var firstWord = withoutAsync.FirstWord();

            if (firstWord.Equals(StartingPhraseFirstWord, comparison))
            {
                var secondWord = withoutAsync.SecondWord();

                if (secondWord.Equals(StartingPhraseSecondWord, comparison))
                {
                    // no issue
                    problematicText = string.Empty;

                    return false;
                }
            }

            problematicText = valueText.FirstWord();

            return true;
        }
    }
}