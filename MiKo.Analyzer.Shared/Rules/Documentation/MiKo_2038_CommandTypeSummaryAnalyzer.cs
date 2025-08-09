using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2038_CommandTypeSummaryAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2038";

        private const string StartingPhrase = Constants.Comments.CommandSummaryStartingPhrase;

        public MiKo_2038_CommandTypeSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is INamedTypeSymbol type && type.IsCommand();

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, Constants.XmlTag.Summary, StartingPhrase);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = string.Empty;
            comparison = StringComparison.Ordinal;

            if (valueText.AsSpan().TrimStart().StartsWith(StartingPhrase))
            {
                return false;
            }

            problematicText = valueText.FirstWord();

            return true;
        }
    }
}