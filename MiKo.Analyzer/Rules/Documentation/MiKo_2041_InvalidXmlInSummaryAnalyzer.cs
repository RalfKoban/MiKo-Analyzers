﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2041_InvalidXmlInSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2041";

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2041_InvalidXmlInSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => from summary in summaries
                                                                                                                    from phrase in Constants.Comments.InvalidSummaryCrefPhrases
                                                                                                                    where summary.Contains(phrase, Comparison)
                                                                                                                    select Issue(symbol, phrase + Constants.Comments.XmlElementEndingTag);
    }
}