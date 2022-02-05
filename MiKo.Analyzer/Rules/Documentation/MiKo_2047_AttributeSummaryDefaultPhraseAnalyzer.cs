using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2047_AttributeSummaryDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2047";

        private static readonly string[] Words =
            {
                "Specifies",
                "Indicates",
                "Defines",
                "Provides",
                "Allows",
                "Represents",
                "Marks",
            };

        private static readonly string StartingPhrases = Words.OrderBy(_ => _).HumanizedConcatenated();

        public MiKo_2047_AttributeSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.InheritsFrom<Attribute>() && base.ShallAnalyze(symbol);

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeSummaryStart(symbol, summaryXml);

        protected override Diagnostic SummaryIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, StartingPhrases);

        protected override Diagnostic SummaryIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            var firstWord = summary.FirstWord();

            foreach (var word in Words)
            {
                if (word.Equals(firstWord, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
            }

            var location = GetLocation(textToken, firstWord);

            return Issue(symbol.Name, location, StartingPhrases);
        }
    }
}