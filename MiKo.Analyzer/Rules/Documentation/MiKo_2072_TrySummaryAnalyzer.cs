using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2072_TrySummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2072";

        internal const string StartingPhrase = "Attempts to";

        internal static readonly string[] Words = { "Try", "Tries" };

        public MiKo_2072_TrySummaryAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeSummaryStart(symbol, summaryXml);

        protected override Diagnostic SummaryIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, StartingPhrase);

        protected override Diagnostic SummaryIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            var trimmed = summary
                          .Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                          .TrimStart();

            var firstWord = trimmed.FirstWord();

            foreach (var word in Words)
            {
                if (word.Equals(firstWord, StringComparison.OrdinalIgnoreCase))
                {
                    var location = GetLocation(textToken, firstWord);

                    return Issue(symbol.Name, location, StartingPhrase);
                }
            }

            return null;
        }
    }
}