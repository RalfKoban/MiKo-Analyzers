using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment) => HasEventSummary(summaries)
                                                                                                                                                                                       ? Enumerable.Empty<Diagnostic>()
                                                                                                                                                                                       : new[] { Issue(symbol, StartingPhraseConcrete, "\"" + EndingPhraseConcrete) };

        private static bool HasEventSummary(IEnumerable<string> summaries)
        {
            foreach (var summary in summaries)
            {
                var trimmedSummary = summary.Without(Constants.Comments.SealedClassPhrase).AsSpan().Trim();

                if (trimmedSummary.StartsWith(StartingPhrase, Comparison))
                {
                    var phrase = trimmedSummary.StartsWith(StartingPhraseConcrete, Comparison)
                                 ? EndingPhraseConcrete
                                 : EndingPhraseMultiple;

                    return trimmedSummary.EndsWith(phrase, Comparison);
                }
            }

            return false;
        }
    }
}