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

        public MiKo_2013_EnumSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEnum() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries)
        {
            if (summaries.None(_ => _.AsSpan().TrimStart().StartsWith(Constants.Comments.EnumStartingPhrase, StringComparison.Ordinal)))
            {
                yield return Issue(symbol, Constants.Comments.EnumStartingPhrase);
            }
        }
    }
}