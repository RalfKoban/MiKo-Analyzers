﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]

    public sealed class MiKo_2019_PassiveVerbSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2019";

        public MiKo_2019_PassiveVerbSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => !symbol.IsNamespace && !symbol.IsEnum() && !symbol.IsException();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(HasPassiveVerb)
                                                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                                                        : new[] { ReportIssue(symbol) };

        private static bool HasPassiveVerb(string summary)
        {
            // get rid of async starting phrase
            summary = summary.Remove(Constants.Comments.AsynchrounouslyStartingPhrase).Trim();

            const StringComparison Comparison = StringComparison.Ordinal;

            var firstSpace = summary.IndexOf(" ", Comparison);
            var firstWord = firstSpace == -1 ? summary : summary.Substring(0, firstSpace);

            return firstWord.EndsWith("s", Comparison) && !firstWord.EndsWithAny(Comparison, "as", "hs", "is", "os", "ss", "us", "xs", "zs");
        }
    }
}