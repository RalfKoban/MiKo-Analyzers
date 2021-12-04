using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]

    public sealed class MiKo_2019_PassiveVerbSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2019";

        private static readonly string[] TwoCharacterEndingsWithS = { "as", "hs", "is", "os", "ss", "us", "xs", "zs" };

        public MiKo_2019_PassiveVerbSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsNamespace is false && symbol.IsEnum() is false && symbol.IsException() is false;

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(HasPassiveVerb)
                                                                                                                        ? Enumerable.Empty<Diagnostic>()
                                                                                                                        : new[] { Issue(symbol) };

        private static bool HasPassiveVerb(string summary)
        {
            var trimmed = summary
                              .Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                              .Without(Constants.Comments.RecursivelyStartingPhrase) // skip over recursively starting phrase
                              .Trim();

            return HasPassiveVerbCore(trimmed);
        }

        private static bool HasPassiveVerbCore(string summary)
        {
            const StringComparison Comparison = StringComparison.Ordinal;

            var firstWord = summary.FirstWord();

            return firstWord.EndsWith("s", Comparison) && firstWord.EndsWithAny(TwoCharacterEndingsWithS, Comparison) is false;
        }
    }
}