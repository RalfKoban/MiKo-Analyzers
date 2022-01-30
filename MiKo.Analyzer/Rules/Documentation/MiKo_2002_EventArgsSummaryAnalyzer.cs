using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2002_EventArgsSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2002";

        private const string StartingPhrase = "Provides data for ";
        private const string EndingPhraseMultiple = " events.";

        private const string StartingPhraseConcrete = StartingPhrase + "the <see cref=\"";
        private const string EndingPhraseConcrete = "/> event.";

        private const StringComparison Comparison = StringComparison.Ordinal;

        public MiKo_2002_EventArgsSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEventArgs() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => HasEventSummary(summaries)
                                                                                                                            ? Enumerable.Empty<Diagnostic>()
                                                                                                                            : new[] { Issue(symbol, StartingPhraseConcrete, "\"" + EndingPhraseConcrete) };

        private static bool HasEventSummary(IEnumerable<string> summaries)
        {
            foreach (var summary in summaries.Select(_ => _.Without(Constants.Comments.SealedClassPhrase).Trim()))
            {
                if (summary.StartsWith(StartingPhrase, Comparison))
                {
                    var phrase = summary.StartsWith(StartingPhraseConcrete, Comparison)
                                     ? EndingPhraseConcrete
                                     : EndingPhraseMultiple;

                    return summary.EndsWith(phrase, Comparison);
                }
            }

            return false;
        }
    }
}