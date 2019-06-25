using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2018_ChecksSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2018";

        private static readonly string[] Comments = { "Check ", "Checks " };

        public MiKo_2018_ChecksSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => symbol.IsNamespace is false && symbol.IsEnum() is false;

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            foreach (var summary in summaries
                                        .Select(_ => _.Remove(Constants.Comments.AsynchrounouslyStartingPhrase).Trim())
                                        .Where(_ => _.StartsWithAny(Comments)))
            {
                return new[] { Issue(symbol, summary.Substring(0, summary.IndexOf(" ", StringComparison.OrdinalIgnoreCase))) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}