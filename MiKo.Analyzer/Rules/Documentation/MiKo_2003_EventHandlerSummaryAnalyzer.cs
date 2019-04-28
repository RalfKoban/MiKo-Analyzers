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

        private const StringComparison Comparison = StringComparison.Ordinal;

        public MiKo_2003_EventHandlerSummaryAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.IsEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            foreach (var summary in summaries)
            {
                // TODO: RKN if (summary.StartsWithAny(Comparison, Constants.Comments.EventHandlerSummaryPhrase)) return Enumerable.Empty<Diagnostic>();
                if (summary.StartsWith(Constants.Comments.EventHandlerSummaryStartingPhrase, Comparison))
                    return Enumerable.Empty<Diagnostic>();
            }

            var phrase = Constants.Comments.EventHandlerSummaryStartingPhrase; // TODO: RKN Constants.Comments.EventHandlerSummaryPhrase[0]
            return new[] { Issue(symbol, phrase) };
        }
    }
}