using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2047_AttributeSummaryDefaultPhraseAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2047";

        private static readonly string StartingPhrases = Constants.Comments.AttributeSummaryStartingPhrase.OrderBy(_ => _).HumanizedConcatenated();

        public MiKo_2047_AttributeSummaryDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is INamedTypeSymbol type && type.InheritsFrom<Attribute>();

        protected override Diagnostic TextStartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, StartingPhrases);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = string.Empty;
            comparison = StringComparison.Ordinal;

            if (valueText.AsSpan().TrimStart().StartsWithAny(Constants.Comments.AttributeSummaryStartingPhrase, comparison))
            {
                return false;
            }

            problematicText = valueText.FirstWord();

            return true;
        }
    }
}