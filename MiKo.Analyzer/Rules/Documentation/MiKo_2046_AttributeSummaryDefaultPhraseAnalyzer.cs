using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2046_AttributeSummaryDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2046";

        private const StringComparison Comparison = StringComparison.Ordinal;

        private static readonly string StartingPhrases = Constants.Comments.AttributeSummaryStartingPhrase.OrderBy(_ => _).HumanizedConcatenated();

        public MiKo_2046_AttributeSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => symbol.InheritsFrom<Attribute>();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(_ => _.StartsWithAny(Comparison, Constants.Comments.AttributeSummaryStartingPhrase))
                                                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                                                        : new[] { ReportIssue(symbol, StartingPhrases) };
    }
}