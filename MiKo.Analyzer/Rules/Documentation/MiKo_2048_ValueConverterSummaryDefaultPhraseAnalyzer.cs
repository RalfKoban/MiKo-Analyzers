using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2048";

        private static readonly string StartingPhrase = Constants.Comments.ValueConverterSummaryStartingPhrase;

        public MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => symbol.IsValueConverter() || symbol.IsMultiValueConverter();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(_ => _.StartsWith(StartingPhrase, StringComparison.Ordinal))
                                                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                                                        : new[] { ReportIssue(symbol, StartingPhrase) };
    }
}