using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            if (summaries.None(_ => _.StartsWith(Constants.Comments.EventHandlerSummaryStartingPhrase, StringComparison.Ordinal)))
            {
                return new[] { Issue(symbol, Constants.Comments.EventHandlerSummaryStartingPhrase) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}