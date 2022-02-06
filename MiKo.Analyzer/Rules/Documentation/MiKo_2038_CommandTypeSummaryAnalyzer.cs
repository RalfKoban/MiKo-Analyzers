using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2038_CommandTypeSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2038";

        internal const string StartingPhrase = "Represents a command that can ";

        public MiKo_2038_CommandTypeSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsCommand() && base.ShallAnalyze(symbol);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxNode node) => Issue(symbol.Name, node, StartingPhrase);

        protected override Diagnostic SummaryStartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            if (summary.StartsWith(StartingPhrase, StringComparison.Ordinal))
            {
                return null;
            }

            return Issue(symbol.Name, textToken, StartingPhrase);
        }
    }
}