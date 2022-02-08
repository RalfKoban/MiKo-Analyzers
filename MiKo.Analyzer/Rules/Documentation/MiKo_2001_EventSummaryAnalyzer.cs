using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2001_EventSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2001";

        public MiKo_2001_EventSummaryAnalyzer() : base(Id, SymbolKind.Event)
        {
        }

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, Constants.Comments.EventSummaryStartingPhrase);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            if (summary.StartsWith(Constants.Comments.EventSummaryStartingPhrase, StringComparison.Ordinal))
            {
                return null;
            }

            var firstWord = summary.FirstWord();
            var location = GetFirstLocation(textToken, firstWord);

            return Issue(symbol.Name, location, Constants.Comments.EventSummaryStartingPhrase);
        }
    }
}