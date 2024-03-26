using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2073";

        private const string StartingPhrase = Constants.Comments.DeterminesWhetherPhrase;

        private static readonly string StartingPhraseFirstWord = StartingPhrase.FirstWord();

        private static readonly string StartingPhraseSecondWord = StartingPhrase.SecondWord();

        public MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Name.StartsWith("Contains", StringComparison.OrdinalIgnoreCase) && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, StartingPhrase);

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            foreach (var summaryXml in summaryXmls)
            {
                yield return AnalyzeTextStart(symbol, summaryXml);
            }
        }

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = StringComparison.OrdinalIgnoreCase;

            var withoutAsync = valueText.Without(Constants.Comments.AsynchrounouslyStartingPhrase); // skip over async starting phrase

            var firstWord = withoutAsync.FirstWord();

            if (firstWord.Equals(StartingPhraseFirstWord, comparison))
            {
                var secondWord = withoutAsync.SecondWord();

                if (secondWord.Equals(StartingPhraseSecondWord, comparison))
                {
                    // no issue
                    problematicText = null;

                    return false;
                }
            }

            problematicText = valueText.FirstWord();

            return true;
        }
    }
}