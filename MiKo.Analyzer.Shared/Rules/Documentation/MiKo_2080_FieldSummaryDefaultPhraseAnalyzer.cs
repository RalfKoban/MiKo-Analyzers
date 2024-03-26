using System;
using System.Collections.Generic;

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

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2080_FieldSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.Field)
        {
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

        protected override Diagnostic StartIssue(ISymbol symbol, Location location)
        {
            var proposal = GetStartingPhrase((IFieldSymbol)symbol);

            return Issue(symbol.Name, location, proposal, CreateStartingPhraseProposal(proposal));
        }

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            foreach (var summaryXml in summaryXmls)
            {
                yield return AnalyzeTextStart(symbol, summaryXml);
            }
        }

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = Comparison;

            var comment = valueText.TrimStart();

            var fieldSymbol = (IFieldSymbol)symbol;
            var phrase = GetStartingPhrase(fieldSymbol);

            if (comment.StartsWith(phrase, Comparison))
            {
                // no issue
                problematicText = null;

                return false;
            }

            // alternative check for enumerables
            if (fieldSymbol.IsConst is false && fieldSymbol.Type.IsEnumerable() && comment.StartsWith(StartingDefaultPhrase, Comparison))
            {
                // no issue
                problematicText = null;

                return false;
            }

            problematicText = comment.FirstWord();

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