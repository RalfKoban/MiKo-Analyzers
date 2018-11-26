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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.IsFactory()
                                                                                               ? base.AnalyzeType(symbol).Concat(symbol.GetMembers().OfType<IMethodSymbol>().SelectMany(AnalyzeMethod)).ToList()
                                                                                               : Enumerable.Empty<Diagnostic>();

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.MethodKind == MethodKind.Ordinary;

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type: return AnalyzeStartingPhrase(type, summaries, Constants.Comments.FactorySummaryPhrase);
                case IMethodSymbol method: return AnalyzeStartingPhrase(symbol, summaries, GetPhrases(method).ToArray());
                default: return Enumerable.Empty<Diagnostic>();
            }
        }

        private IEnumerable<string> GetPhrases(IMethodSymbol symbol) => symbol.ReturnType.IsEnumerable() ? GetCollectionPhrases(symbol) : GetSimplePhrases(symbol);

        private IEnumerable<string> GetSimplePhrases(IMethodSymbol symbol) => GetPhrases(symbol.ReturnType, Constants.Comments.FactoryCreateMethodSummaryStartingPhrase);

        private IEnumerable<string> GetCollectionPhrases(IMethodSymbol symbol)
        {
            TryGetGenericArgumentCount(symbol.ReturnType, out var count);
            if (count <= 0) return GetSimplePhrases(symbol);

            // enhance for generic collections
            var startingPhrases = Constants.Comments.FactoryCreateCollectionMethodSummaryStartingPhrase;

            return TryGetGenericArgumentType(symbol.ReturnType, out var genericArgument, count - 1)
                       ? GetPhrases(genericArgument, startingPhrases)
                       : startingPhrases.Select(_ => string.Format(_, genericArgument.ToString()));
        }

        private IEnumerable<string> GetPhrases(ITypeSymbol symbolReturnType, string[] startingPhrases)
        {
            var returnType = symbolReturnType.ToString();

            TryGetGenericArgumentCount(symbolReturnType, out var count);
            if (count <= 0) return startingPhrases.Select(_ => string.Format(_, returnType));

            var ts = GetGenericArgumentsAsTs(symbolReturnType);

            var length = returnType.IndexOf('<'); // just until the first one

            var returnTypeWithTs = returnType.Substring(0, length) + "{" + ts + "}";
            var returnTypeWithGenericCount = returnType.Substring(0, length) + '`' + count;

            return Enumerable.Empty<string>()
                             .Concat(startingPhrases.Select(_ => string.Format(_, returnTypeWithTs))) // for the phrases to show to the user
                             .Concat(startingPhrases.Select(_ => string.Format(_, returnTypeWithGenericCount))); // for the real check
        }

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, IEnumerable<string> comments, params string[] phrases)
        {
            return comments.Any(_ => phrases.Any(__ => _.StartsWith(__, StringComparison.Ordinal)))
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(symbol, phrases.First()) };
        }
    }
}