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

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.NamedType, SymbolKind.Property);

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => !symbol.IsNamespace && !symbol.IsEnum();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var phrases = symbol.Kind == SymbolKind.Field
                          ? Constants.Comments.MeaninglessFieldStartingPhrase
                          : Constants.Comments.MeaninglessStartingPhrase;

            var symbolNames = GetSelfSymbolNames(symbol);
            foreach (var summary in summaries)
            {
                foreach (var phrase in symbolNames.Where(_ => summary.StartsWith(_, Comparison)))
                {
                    return ReportIssueStartingPhrase(symbol, phrase);
                }

                foreach (var phrase in phrases.Where(_ => summary.StartsWith(_, Comparison)))
                {
                    return ReportIssueStartingPhrase(symbol, phrase);
                }

                if (summary.StartsWith(Constants.Comments.XmlElementStartingTag, Comparison))
                {
                    var index = summary.IndexOf(Constants.Comments.XmlElementEndingTag, Comparison);
                    var phrase = index > 0 ? summary.Substring(0, index + 2) : Constants.Comments.XmlElementStartingTag;
                    return ReportIssueStartingPhrase(symbol, phrase);
                }

                foreach (var phrase in Constants.Comments.MeaninglessPhrase.Where(_ => summary.Contains(_, Comparison)))
                {
                    return ReportIssueContainsPhrase(symbol, phrase);
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> ReportIssueContainsPhrase(ISymbol symbol, string phrase) => new[] { ReportIssue(symbol, "contains", phrase.HumanizedTakeFirst(30)) };

        private IEnumerable<Diagnostic> ReportIssueStartingPhrase(ISymbol symbol, string phrase) => new[] { ReportIssue(symbol, "starts with", phrase.HumanizedTakeFirst(30)) };

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
    }
}