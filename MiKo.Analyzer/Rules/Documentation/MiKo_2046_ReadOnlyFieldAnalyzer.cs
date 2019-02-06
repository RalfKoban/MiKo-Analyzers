using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2046_ReadOnlyFieldAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2046";

        private const StringComparison Comparison = StringComparison.Ordinal;

        public MiKo_2046_ReadOnlyFieldAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyzeField(IFieldSymbol symbol)
        {
            if (symbol.IsReadOnly)
            {
                switch (symbol.DeclaredAccessibility)
                {
                    case Accessibility.Public:
                    case Accessibility.Protected:
                        return !symbol.ContainingType.IsTestClass();
                }
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(_ => _.EndsWith(Constants.Comments.FieldIsReadOnly, Comparison))
                                                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                                                        : new[] { ReportIssue(symbol, Constants.Comments.FieldIsReadOnly) };
    }
}