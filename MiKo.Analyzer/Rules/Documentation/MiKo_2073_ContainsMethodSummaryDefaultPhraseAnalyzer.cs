using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2073";

        internal const string StartingPhrase = Constants.Comments.DeterminesWhetherPhrase;

        public MiKo_2073_ContainsMethodSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Name.StartsWith("Contains", StringComparison.OrdinalIgnoreCase) && base.ShallAnalyze(symbol);

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeSummaryStart(symbol, summaryXml);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, StartingPhrase);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            var trimmed = summary
                          .Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                          .Trim();

            if (trimmed.StartsWith(StartingPhrase, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var location = GetLocation(textToken, summary.FirstWord(), StringComparison.OrdinalIgnoreCase);

            return Issue(symbol.Name, location, StartingPhrase);
        }
    }
}