using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2001_EventSummaryAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2001";

        private const string StartingPhrase = Constants.Comments.EventSummaryStartingPhrase;

        public MiKo_2001_EventSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol.Kind == SymbolKind.Event;

        protected override Diagnostic TextStartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, StartingPhrase);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = string.Empty;
            comparison = StringComparison.Ordinal;

            var text = valueText.AsSpan().TrimStart();

            if (text.StartsWith(StartingPhrase, comparison))
            {
                return false;
            }

            problematicText = text.FirstWord().ToString();

            return true;
        }
    }
}