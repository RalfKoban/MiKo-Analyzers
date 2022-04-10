using System;

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

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, StartingPhrase);

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText.TrimStart();

            if (summary.StartsWith(StartingPhrase, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, StartingPhrase);
        }
    }
}