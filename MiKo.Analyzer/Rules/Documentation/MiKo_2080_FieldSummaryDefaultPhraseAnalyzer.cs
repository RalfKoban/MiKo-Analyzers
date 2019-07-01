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

        protected override bool ShallAnalyzeField(IFieldSymbol symbol)
        {
            if (symbol.ContainingType.IsEnum())
            {
                return false;
            }

            if (symbol.Type.IsDependencyProperty())
            {
                return false; // validated by rule MiKo_2017
            }

            return true;
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var fieldSymbol = (IFieldSymbol)symbol;
            var type = fieldSymbol.Type;

            var phrase = fieldSymbol.IsConst || type.SpecialType != SpecialType.System_Boolean
                             ? StartingDefaultPhrase
                             : StartingBooleanDefaultPhrase;

            if (summaries.Any(_ => _.StartsWith(phrase, Comparison)))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            // alternative check
            if (fieldSymbol.IsConst is false && type.IsEnumerable())
            {
                phrase = StartingEnumerableDefaultPhrase;

                if (summaries.Any(_ => _.StartsWith(phrase, Comparison)))
                {
                    return Enumerable.Empty<Diagnostic>();
                }
            }

            return new[] { Issue(symbol, phrase) };
        }
    }
}