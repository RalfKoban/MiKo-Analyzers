﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2012_TypeMeaninglessSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2012";

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2012_TypeMeaninglessSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => !symbol.IsNamespace && !symbol.IsEnum();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var symbolNames = new List<string>(((INamedTypeSymbol)symbol).AllInterfaces.Select(_ => _.Name)) { symbol.Name };
            foreach (var summary in summaries)
            {
                foreach (var phrase in symbolNames.Where(phrase => summary.StartsWith(phrase, Comparison)))
                {
                    return ReportIssue(symbol, summary, phrase);
                }

                foreach (var phrase in Constants.Comments.MeaninglessTypeStartingPhrase.Where(phrase => summary.StartsWith(phrase, Comparison)))
                {
                    return ReportIssue(symbol, summary, phrase);
                }

                if (summary.StartsWith("<", Comparison))
                {
                    var index = summary.IndexOf("/>", Comparison);
                    var phrase = index > 0 ? summary.Substring(0, index + 2) : "<";
                    return ReportIssue(symbol, summary, phrase);
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> ReportIssue(ISymbol symbol, string summary, string defaultPhrase)
        {
            var index = summary.IndexOfTimes(7, ' ');
            var phrase = index > 0 ? summary.Substring(0, index) : defaultPhrase;
            return new[] { ReportIssue(symbol, phrase) };
        }
    }
}