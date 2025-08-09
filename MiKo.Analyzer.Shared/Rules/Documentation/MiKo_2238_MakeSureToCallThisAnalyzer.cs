using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2238_MakeSureToCallThisAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2238";

        private const string Phrase = "Make sure to call this";

        public MiKo_2238_MakeSureToCallThisAnalyzer() : base(Id)
        {
        }

        protected override bool ConsiderEmptyTextAsIssue(ISymbol symbol) => false;

        protected override Diagnostic NonTextStartIssue(ISymbol symbol, SyntaxNode node) => null; // this is no issue as we do not start with any word

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = string.Empty;
            comparison = StringComparison.Ordinal;

            var text = valueText.AsSpan().TrimStart();

            if (text.StartsWith(Phrase))
            {
                problematicText = Phrase;

                return true;
            }

            return false;
        }
    }
}