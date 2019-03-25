using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2207_SummaryShallBeShortAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2207";

        private const int MaxAllowedWhitespaces = 50;

        public MiKo_2207_SummaryShallBeShortAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(HasIssue)
                                                                                                                        ? new[] { ReportIssue(symbol) }
                                                                                                                        : Enumerable.Empty<Diagnostic>();

        private static bool HasIssue(string summary)
        {
            var whitespaces = 0;

            foreach (var c in summary.Replace(" - ", " ").Replace(" />", "/>").Replace(" cref=", "cref=").Trim())
            {
                if (c.IsWhiteSpace())
                    whitespaces++;

                if (whitespaces > MaxAllowedWhitespaces)
                    return true;
            }

            return false;
        }
    }
}