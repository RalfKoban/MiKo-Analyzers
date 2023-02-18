using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2081_ReadOnlyFieldAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2081";

        private const StringComparison Comparison = StringComparison.Ordinal;

        public MiKo_2081_ReadOnlyFieldAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol)
        {
            if (symbol.IsReadOnly)
            {
                switch (symbol.DeclaredAccessibility)
                {
                    case Accessibility.Public:
                    case Accessibility.Protected:
                        return symbol.ContainingType.IsTestClass() is false;
                }
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            if (summaries.None(_ => _.EndsWith(Constants.Comments.FieldIsReadOnly, Comparison)))
            {
                return new[] { Issue(symbol, Constants.Comments.FieldIsReadOnly) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}