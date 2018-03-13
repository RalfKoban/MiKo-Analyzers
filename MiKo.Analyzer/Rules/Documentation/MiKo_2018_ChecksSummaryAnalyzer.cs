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

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2018_ChecksSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        public override void Initialize(AnalysisContext context)
        {
            Initialize(context, SymbolKind.Method);
            Initialize(context, SymbolKind.NamedType);
            Initialize(context, SymbolKind.Property);
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => !symbol.IsNamespace && !symbol.IsEnum();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            foreach (var summary in summaries)
            {
                if (summary.StartsWithAny(Comparison, "Check ", "Checks "))
                {
                    return new[] { ReportIssue(symbol, summary.Substring(0, summary.IndexOf(" ", Comparison))) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}