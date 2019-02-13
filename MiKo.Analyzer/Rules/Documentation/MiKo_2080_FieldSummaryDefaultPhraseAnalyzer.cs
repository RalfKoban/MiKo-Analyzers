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
        private const string StartingBooleanDefaultPhrase = "Indicates whether ";

        private const StringComparison Comparison = StringComparison.Ordinal;

        public MiKo_2080_FieldSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyzeField(IFieldSymbol symbol) => !symbol.ContainingType.IsEnum();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var type = ((IFieldSymbol)symbol).Type;

            var phrase = type.SpecialType == SpecialType.System_Boolean
                             ? StartingBooleanDefaultPhrase
                             : StartingDefaultPhrase;

            if (summaries.Any(_ => _.StartsWith(phrase, Comparison)))
                return Enumerable.Empty<Diagnostic>();

            // alternative check
            if (type.IsEnumerable())
            {
                phrase = StartingEnumerableDefaultPhrase;

                if (summaries.Any(_ => _.StartsWith(phrase, Comparison)))
                    return Enumerable.Empty<Diagnostic>();
            }

            return new[] { ReportIssue(symbol, phrase) };
        }
    }
}