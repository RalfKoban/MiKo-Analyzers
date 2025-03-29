using System;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2072_TrySummaryAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2072";

        public MiKo_2072_TrySummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol.Kind == SymbolKind.Method;

        protected override bool ConsiderEmptyTextAsIssue(ISymbol symbol) => false;

        protected override Diagnostic NonTextStartIssue(ISymbol symbol, SyntaxNode node) => null; // this is no issue as we do not start with any word

        protected override Diagnostic TextStartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, Constants.Comments.TryStartingPhrase);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = StringComparison.Ordinal;

            var builder = valueText.AsCachedBuilder()
                                   .Without(Constants.Comments.AsynchronouslyStartingPhrase); // skip over async starting phrase

            var firstWord = builder.FirstWord(out _);

            StringBuilderCache.Release(builder);

            if (firstWord.EqualsAny(Constants.Comments.TryWords))
            {
                problematicText = firstWord;

                return true;
            }

            problematicText = null;

            return false;
        }
    }
}