using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2016_AsyncMethodDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2016";

        private const string Phrase = Constants.Comments.AsynchrounouslyStartingPhrase;

        public MiKo_2016_AsyncMethodDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsAsyncTaskBased() && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, Phrase);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            if (summary.StartsWith(Phrase, StringComparison.Ordinal))
            {
                var location = GetFirstLocation(textToken, Phrase);

                return Issue(symbol.Name, location, Phrase);
            }

            return null;
        }
    }
}