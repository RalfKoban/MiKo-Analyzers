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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            if (!symbol.IsFactory()) return Enumerable.Empty<Diagnostic>();

            return base.AnalyzeType(symbol).Concat(symbol.GetMembers().OfType<IMethodSymbol>().SelectMany(AnalyzeMethod)).ToList();
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type: return AnalyzeTypeSummary(type, summaries);
                case IMethodSymbol method: return AnalyzeMethodSummary(method, summaries);
                default: return Enumerable.Empty<Diagnostic>();
            }
        }

        private IEnumerable<Diagnostic> AnalyzeTypeSummary(ITypeSymbol symbol, IEnumerable<string> summaries)
        {
            return AnalyzeStartingPhrase(symbol, summaries, Constants.Comments.FactorySummaryPhrase);
        }

        private IEnumerable<Diagnostic> AnalyzeMethodSummary(IMethodSymbol symbol, IEnumerable<string> summaries)
        {
            var phrases = Constants.Comments.FactoryCreateMethodSummaryStartingPhrase.Select(_ => string.Format(_, symbol.ReturnType)).ToArray();

            return AnalyzeStartingPhrase(symbol, summaries, phrases);
        }

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, IEnumerable<string> comments, params string[] phrases)
        {
            return comments.Any(_ => phrases.Any(__ => _.StartsWith(__, StringComparison.Ordinal)))
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(symbol, phrases.First()) };
        }
    }
}