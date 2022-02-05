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
        private const string StartingGuidDefaultPhrase = "The unique identifier for ";

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

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeSummaryStart(symbol, summaryXml);

        protected override Diagnostic SummaryIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, GetStartingPhrase((IFieldSymbol)symbol));

        protected override Diagnostic SummaryIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var fieldSymbol = (IFieldSymbol)symbol;

            var phrase = GetStartingPhrase(fieldSymbol);

            var summary = textToken.ValueText;

            if (summary.StartsWith(phrase, StringComparison.Ordinal))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, phrase);
        }
    }
}