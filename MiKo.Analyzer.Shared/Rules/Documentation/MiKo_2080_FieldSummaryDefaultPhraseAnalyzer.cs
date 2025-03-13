using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2080_FieldSummaryDefaultPhraseAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2080";

        private const string StartingDefaultPhrase = "The ";
        private const string StartingEnumerableDefaultPhrase = "Contains ";
        private const string StartingBooleanDefaultPhrase = "Indicates whether ";
        private const string StartingGuidDefaultPhrase = "The unique identifier for ";

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2080_FieldSummaryDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            if (symbol is IFieldSymbol field)
            {
                if (field.ContainingType.IsEnum())
                {
                    return false;
                }

                if (field.Type.IsDependencyProperty())
                {
                    return false; // validated by rule MiKo_2017
                }

                if (field.Type.IsRoutedEvent())
                {
                    return false; // validated by rule MiKo_2006
                }

                return true;
            }

            return false;
        }

        protected override Diagnostic StartIssue(ISymbol symbol, Location location)
        {
            var proposal = GetStartingPhrase((IFieldSymbol)symbol);

            return Issue(symbol.Name, location, proposal, CreateStartingPhraseProposal(proposal));
        }

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = Comparison;

            var comment = valueText.AsSpan().TrimStart();

            var fieldSymbol = (IFieldSymbol)symbol;
            var phrase = GetStartingPhrase(fieldSymbol);

            if (comment.StartsWith(phrase, Comparison))
            {
                // no issue
                problematicText = string.Empty;

                return false;
            }

            // alternative check for enumerables
            if (fieldSymbol.IsConst is false && fieldSymbol.Type.IsEnumerable() && comment.StartsWith(StartingDefaultPhrase, Comparison))
            {
                // no issue
                problematicText = null;

                return false;
            }

            problematicText = comment.FirstWord().ToString();

            return true;
        }

        private static string GetStartingPhrase(IFieldSymbol symbol)
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
    }
}