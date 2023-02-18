using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        private const string StartingGuidDefaultPhrase = "The unique identifier for ";

        private const StringComparison Comparison = StringComparison.Ordinal;

        public MiKo_2080_FieldSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        internal static string GetStartingPhrase(IFieldSymbol symbol)
        {
            if (symbol.IsConst)
            {
                return StartingDefaultPhrase;
            }

            var type = symbol.Type;

            if (type.IsBoolean())
            {
                return StartingBooleanDefaultPhrase;
            }

            if (type.IsGuid())
            {
                return StartingGuidDefaultPhrase;
            }

            if (type.IsEnumerable())
            {
                return StartingEnumerableDefaultPhrase;
            }

            return StartingDefaultPhrase;
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol)
        {
            if (symbol.ContainingType.IsEnum())
            {
                return false;
            }

            if (symbol.Type.IsDependencyProperty())
            {
                return false; // validated by rule MiKo_2017
            }

            if (symbol.Type.IsRoutedEvent())
            {
                return false; // validated by rule MiKo_2006
            }

            return base.ShallAnalyze(symbol);
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            var fieldSymbol = (IFieldSymbol)symbol;

            var phrase = GetStartingPhrase(fieldSymbol);

            if (summaries.Any(_ => _.StartsWith(phrase, Comparison)))
            {
                yield break;
            }

            // alternative check for enumerables
            if (fieldSymbol.IsConst is false && fieldSymbol.Type.IsEnumerable())
            {
                if (summaries.Any(_ => _.StartsWith(StartingDefaultPhrase, Comparison)))
                {
                    yield break;
                }
            }

            yield return Issue(symbol, phrase);
        }
    }
}