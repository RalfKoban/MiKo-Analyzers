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

        private static readonly string[] StartingDefaultPhrases = { "The " };
        private static readonly string[] StartingEnumerableDefaultPhrases = { "Contains ", "The " };
        private static readonly string[] StartingBooleanDefaultPhrases = { "Indicates whether " };
        private static readonly string[] StartingGuidDefaultPhrases = { "The unique identifier for " };

        public MiKo_2080_FieldSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        internal static string[] GetStartingPhrases(IFieldSymbol symbol)
        {
            if (symbol.IsConst)
            {
                return StartingDefaultPhrases;
            }

            var type = symbol.Type;

            if (type.IsBoolean())
            {
                return StartingBooleanDefaultPhrases;
            }

            if (type.IsGuid())
            {
                return StartingGuidDefaultPhrases;
            }

            if (type.IsEnumerable())
            {
                return StartingEnumerableDefaultPhrases;
            }

            return StartingDefaultPhrases;
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

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, GetStartingPhrases((IFieldSymbol)symbol));

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var fieldSymbol = (IFieldSymbol)symbol;

            var phrases = GetStartingPhrases(fieldSymbol);

            var summary = textToken.ValueText.TrimStart();

            if (summary.StartsWithAny(phrases, StringComparison.Ordinal))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, phrases[0]);
        }
    }
}