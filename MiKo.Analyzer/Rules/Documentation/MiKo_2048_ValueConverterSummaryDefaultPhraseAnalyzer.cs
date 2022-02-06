using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2048";

        private const string StartingPhrase = Constants.Comments.ValueConverterSummaryStartingPhrase;

        public MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => (symbol.IsValueConverter() || symbol.IsMultiValueConverter()) && base.ShallAnalyze(symbol);

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeSummaryStart(symbol, summaryXml);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, StartingPhrase);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            if (summary.StartsWith(StartingPhrase, StringComparison.Ordinal))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, StartingPhrase);
        }
    }
}