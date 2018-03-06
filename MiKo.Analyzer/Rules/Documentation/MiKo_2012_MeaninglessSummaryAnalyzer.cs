using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2012_MeaninglessSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2012";

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2012_MeaninglessSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        public override void Initialize(AnalysisContext context)
        {
            Initialize(context, SymbolKind.Event);
            Initialize(context, SymbolKind.Field);
            Initialize(context, SymbolKind.Method);
            Initialize(context, SymbolKind.NamedType);
            Initialize(context, SymbolKind.Property);
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => !symbol.IsNamespace && !symbol.IsEnum();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var phrases = symbol.Kind == SymbolKind.Field
                          ? Constants.Comments.MeaninglessFieldStartingPhrase
                          : Constants.Comments.MeaninglessStartingPhrase;

            var symbolNames = GetSelfSymbolNames(symbol);
            foreach (var summary in summaries)
            {
                foreach (var phrase in symbolNames.Where(phrase => summary.StartsWith(phrase, Comparison)))
                {
                    return ReportIssue(symbol, summary, phrase);
                }

                foreach (var phrase in phrases.Where(phrase => summary.StartsWith(phrase, Comparison)))
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

        private static IEnumerable<string> GetSelfSymbolNames(ISymbol symbol)
        {
            var names = new List<string> { symbol.Name };

            switch (symbol)
            {
                case INamedTypeSymbol s:
                    {
                        names.AddRange(s.AllInterfaces.Select(_ => _.Name));
                        break;
                    }
                case ISymbol s:
                    {
                        names.Add(s.ContainingType.Name);
                        names.AddRange(s.ContainingType.AllInterfaces.Select(_ => _.Name));
                        break;
                    }
            }

            return names.Select(_ => _ + " ");
        }

        private IEnumerable<Diagnostic> ReportIssue(ISymbol symbol, string summary, string defaultPhrase)
        {
            var index = summary.IndexOfTimes(6, ' ');
            var phrase = index > 0 ? summary.Substring(0, index) + "..." : defaultPhrase;
            return new[] { ReportIssue(symbol, phrase) };
        }
    }
}