using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2003_EventHandlerSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2003";

        public MiKo_2003_EventHandlerSummaryAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsEventHandler() && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, Constants.Comments.EventHandlerSummaryStartingPhrase);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var text = textToken.ValueText.TrimStart();

            if (text.StartsWith(Constants.Comments.EventHandlerSummaryStartingPhrase, StringComparison.Ordinal))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, Constants.Comments.EventHandlerSummaryStartingPhrase);
        }
    }
}