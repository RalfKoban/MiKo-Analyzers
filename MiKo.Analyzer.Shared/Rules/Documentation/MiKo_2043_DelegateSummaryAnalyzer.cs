using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2043_DelegateSummaryAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2043";

        private const string StartingPhrase = Constants.Comments.DelegateSummaryStartingPhrase;

        public MiKo_2043_DelegateSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is INamedTypeSymbol type && type.TypeKind == TypeKind.Delegate;

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, Constants.XmlTag.Summary, StartingPhrase);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = null;
            comparison = StringComparison.Ordinal;

            if (valueText.AsSpan().TrimStart().StartsWith(StartingPhrase, comparison))
            {
                return false;
            }

            problematicText = valueText.FirstWord();

            return true;
        }
    }
}