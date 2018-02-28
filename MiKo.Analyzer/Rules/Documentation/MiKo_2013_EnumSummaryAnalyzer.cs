using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2013_EnumSummaryAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2013";

        public MiKo_2013_EnumSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => symbol.IsEnum();

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, string commentXml) => AnalyzeSummary(symbol, commentXml);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            const string Phrase = "Defines values that specify ";
            return summaries.Any(_ => _.TrimStart().StartsWith(Phrase, StringComparison.Ordinal))
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(symbol, Phrase) };
        }
    }
}