using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            if (summaries.None(_ => _.StartsWith(Constants.Comments.EventSummaryStartingPhrase, StringComparison.Ordinal)))
            {
                return new[] { Issue(symbol, Constants.Comments.EventSummaryStartingPhrase) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}