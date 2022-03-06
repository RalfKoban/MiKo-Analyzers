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

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, StartingPhrase);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            var firstWord = summary.Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                                   .FirstWord();

            foreach (var word in Words)
            {
                if (word.Equals(firstWord, StringComparison.OrdinalIgnoreCase))
                {
                    var location = GetFirstLocation(textToken, firstWord);

                    return Issue(symbol.Name, location, StartingPhrase);
                }
            }

            return null;
        }
    }
}