using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2060_FactoryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2060";

        public MiKo_2060_FactoryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type:
                    return type.IsFactory();

                case IMethodSymbol method:
                    return method.ContainingType.IsFactory()
                        && method.MethodKind == MethodKind.Ordinary
                        && method.IsPubliclyVisible()
                        && method.ReturnsVoid is false
                        && method.ReturnType.SpecialType != SpecialType.System_Boolean;

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<IReadOnlyCollection<string>> summaries)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type: return AnalyzeStartingPhrase(type, summaryXmls, summaries.Value, Constants.Comments.FactorySummaryPhrase);
                case IMethodSymbol method: return AnalyzeStartingPhrase(method, summaryXmls, summaries.Value, GetPhrases(method).ToArray());
                default: return Array.Empty<Diagnostic>();
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
                   : startingPhrases.Select(_ => _.FormatWith(argumentType));
        }

        private Diagnostic[] AnalyzeStartingPhrase(ISymbol symbol, IReadOnlyList<XmlElementSyntax> summaryXmls, IEnumerable<string> summaries, params string[] phrases)
        {
            if (summaries.None(_ => phrases.Exists(__ => _.StartsWith(__, StringComparison.Ordinal))))
            {
                return new[] { Issue(symbol.Name, summaryXmls[0].GetContentsLocation(), phrases[0]) };
            }

            // fitting comment
            return Array.Empty<Diagnostic>();
        }
    }
}