using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2071_EnumMethodSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2071";

        private static readonly string[] BooleanPhrases = (from term1 in new[] { " indicating ", " indicates ", " indicate " }
                                                           from term2 in new[] { "whether ", "if " }
                                                           select string.Concat(term1, term2))
                                                          .ToArray();

        public MiKo_2071_EnumMethodSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.ReturnType.IsEnum();

        protected override bool ShallAnalyzeProperty(IPropertySymbol symbol) => symbol.GetReturnType()?.IsEnum() == true;

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => from summary in summaries
                                                                                                                    from phrase in BooleanPhrases
                                                                                                                    where summary.Contains(phrase, StringComparison.OrdinalIgnoreCase)
                                                                                                                    select Issue(symbol, phrase.Trim());
    }
}