using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeSummaryStart(symbol, summaryXml);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, Constants.Comments.EventHandlerSummaryStartingPhrase);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            if (summary.StartsWith(Constants.Comments.EventHandlerSummaryStartingPhrase, StringComparison.Ordinal))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, Constants.Comments.EventHandlerSummaryStartingPhrase);
        }
    }
}