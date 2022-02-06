using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2013_EnumSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2013";

        internal const string StartingPhrase = "Defines values that specify ";

        public MiKo_2013_EnumSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEnum() && base.ShallAnalyze(symbol);

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => AnalyzeSummaryStart(symbol, summaryXml);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, StartingPhrase);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            if (summary.StartsWith(StartingPhrase, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, StartingPhrase);
        }
    }
}