using System;
using System.Collections.Generic;

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

        public MiKo_2002_EventArgsSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is INamedTypeSymbol s && s.IsEventArgs();

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<string[]> summaries)
        {
            return HasEventSummary(summaries.Value)
                   ? Array.Empty<Diagnostic>()
                   : new[] { Issue(summaryXmls[0].GetContentsLocation(), StartingPhraseConcrete, "\"" + EndingPhraseConcrete) };
        }

        private static bool HasEventSummary(string[] summaries)
        {
            var summariesLength = summaries.Length;

            if (summariesLength > 0)
            {
                for (var index = 0; index < summariesLength; index++)
                {
                    var summary = summaries[index];

                    var trimmedSummary = summary.Without(Constants.Comments.SealedClassPhrase).AsSpan().Trim();

                    if (trimmedSummary.StartsWith(StartingPhrase, Comparison))
                    {
                        var phrase = trimmedSummary.StartsWith(StartingPhraseConcrete, Comparison)
                                     ? EndingPhraseConcrete
                                     : EndingPhraseMultiple;

                        return trimmedSummary.EndsWith(phrase, Comparison);
                    }
                }
            }

            return false;
        }
    }
}