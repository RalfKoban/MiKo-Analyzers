using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2080_FieldSummaryDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2080";

        private const string StartingDefaultPhrase = "The ";
        private const string StartingEnumerableDefaultPhrase = "Contains ";

        private const StringComparison Comparison = StringComparison.Ordinal;

        public MiKo_2080_FieldSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var phrase = GetMatchingPhrase(symbol);

            return summaries.Any(_ => _.StartsWith(phrase, Comparison))
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(symbol, phrase) };
        }

        private static string GetMatchingPhrase(ISymbol symbol) => ((IFieldSymbol)symbol).Type.IsEnumerable()
                                                                       ? StartingEnumerableDefaultPhrase
                                                                       : StartingDefaultPhrase;
    }
}