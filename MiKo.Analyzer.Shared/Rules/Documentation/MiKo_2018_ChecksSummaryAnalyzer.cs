using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2018_ChecksSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2018";

        internal const string StartingPhrase = Constants.Comments.DeterminesWhetherPhrase;

        private static readonly string[] WrongPhrases = { "Check ", "Checks ", "Test ", "Tests ", "Determines if " };

        public MiKo_2018_ChecksSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsNamespace is false && symbol.IsEnum() is false && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var summary in summaries)
            {
                var issue = AnalyzeSummary(symbol, summary);
                if (issue != null)
                {
                    yield return issue;
                }
            }
        }

        private Diagnostic AnalyzeSummary(ISymbol symbol, string summary)
        {
            var trimmedSummary = summary.Without(Constants.Comments.AsynchrounouslyStartingPhrase).AsSpan().Trim();

            foreach (var wrongPhrase in WrongPhrases)
            {
                if (trimmedSummary.StartsWith(wrongPhrase, StringComparison.OrdinalIgnoreCase))
                {
                    return Issue(symbol, wrongPhrase, StartingPhrase);
                }
            }

            return null;
        }
    }
}