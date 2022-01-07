﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2003_EventHandlerSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2003";

        public MiKo_2003_EventHandlerSummaryAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(_ => _.StartsWith(Constants.Comments.EventHandlerSummaryStartingPhrase, StringComparison.Ordinal))
                                                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                                                        : new[] { Issue(symbol, Constants.Comments.EventHandlerSummaryStartingPhrase) };
    }
}