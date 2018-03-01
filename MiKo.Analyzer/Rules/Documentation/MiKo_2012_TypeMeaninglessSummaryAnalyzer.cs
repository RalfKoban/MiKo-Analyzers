using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2012_TypeMeaninglessSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2012";
        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2012_TypeMeaninglessSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => !symbol.IsNamespace && !symbol.IsEnum();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var symbolNames = new List<string>(((INamedTypeSymbol)symbol).AllInterfaces.Select(_ => _.Name)) { symbol.Name };
            foreach (var summary in summaries)
            {
                foreach (var symbolName in symbolNames)
                {
                    if (summary.StartsWith(symbolName, Comparison)) return new[] { ReportIssue(symbol, symbolName) };
                }

                foreach (var phrase in Constants.Comments.MeaninglessTypeStartingPhrase)
                {
                    if (summary.StartsWith(phrase, Comparison)) return new[] { ReportIssue(symbol, phrase) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}