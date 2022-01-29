using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]

    public sealed class MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2019";

        public MiKo_2019_3rdPersonSingularVerbSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsNamespace is false && symbol.IsEnum() is false && symbol.IsException() is false && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(HasThirdPersonSingularVerb)
                                                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                                                        : new[] { Issue(symbol) };

        private static bool HasThirdPersonSingularVerb(string summary)
        {
            var trimmed = summary
                              .Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                              .Without(Constants.Comments.RecursivelyStartingPhrase) // skip over recursively starting phrase
                              .Trim();

            var firstWord = trimmed.FirstWord();

            return Verbalizer.IsThirdPersonSingularVerb(firstWord);
        }
    }
}