using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2060_FactoryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2060";

        public MiKo_2060_FactoryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsFactory();

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.MethodKind == MethodKind.Ordinary;

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation, string commentXml) => base.AnalyzeType(symbol, compilation, commentXml).Concat(symbol.GetMethods().SelectMany(_ => AnalyzeMethod(_, compilation))).ToList();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type: return AnalyzeStartingPhrase(type, summaries, Constants.Comments.FactorySummaryPhrase);
                case IMethodSymbol method: return AnalyzeStartingPhrase(symbol, summaries, GetPhrases(method).ToArray());
                default: return Enumerable.Empty<Diagnostic>();
            }
        }

        private static IEnumerable<string> GetPhrases(IMethodSymbol symbol) => symbol.ReturnType.IsEnumerable() ? GetCollectionPhrases(symbol) : GetSimplePhrases(symbol);

        private static IEnumerable<string> GetSimplePhrases(IMethodSymbol symbol) => GetStartingPhrases(symbol.ReturnType, Constants.Comments.FactoryCreateMethodSummaryStartingPhrase);

        private static IEnumerable<string> GetCollectionPhrases(IMethodSymbol symbol)
        {
            symbol.ReturnType.TryGetGenericArgumentCount(out var count);

            if (count <= 0)
            {
                return GetSimplePhrases(symbol);
            }

            // enhance for generic collections
            var startingPhrases = Constants.Comments.FactoryCreateCollectionMethodSummaryStartingPhrase;

            return symbol.ReturnType.TryGetGenericArgumentType(out var argumentType, count - 1)
                       ? GetStartingPhrases(argumentType, startingPhrases)
                       : startingPhrases.Select(_ => string.Format(_, argumentType));
        }

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, IEnumerable<string> comments, params string[] phrases)
        {
            return comments.Any(_ => phrases.Any(__ => _.StartsWith(__, StringComparison.Ordinal)))
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol, phrases.First()) };
        }
    }
}