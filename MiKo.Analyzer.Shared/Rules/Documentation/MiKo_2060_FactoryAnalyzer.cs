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

        public MiKo_2060_FactoryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.MethodKind == MethodKind.Ordinary
                                                                   && symbol.IsPubliclyVisible()
                                                                   && symbol.ReturnsVoid is false
                                                                   && symbol.ReturnType.SpecialType != SpecialType.System_Boolean
                                                                   && base.ShallAnalyze(symbol);

        // overridden because we want to inspect the methods of the type as well
        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.IsFactory())
            {
                return AnalyzeAll(symbol, compilation);
            }

            return Array.Empty<Diagnostic>();
        }

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            switch (symbol)
            {
                case INamedTypeSymbol type: return AnalyzeStartingPhrase(type, summaries, comment, Constants.Comments.FactorySummaryPhrase);
                case IMethodSymbol method: return AnalyzeStartingPhrase(symbol, summaries, comment, GetPhrases(method).ToArray());
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

        private Diagnostic[] AnalyzeStartingPhrase(ISymbol symbol, IEnumerable<string> comments, DocumentationCommentTriviaSyntax comment, params string[] phrases)
        {
            if (comments.None(_ => phrases.Exists(__ => _.StartsWith(__, StringComparison.Ordinal))))
            {
                var summaries = comment.GetSummaryXmls();

                return new[] { Issue(symbol.Name, summaries[0].GetContentsLocation(), phrases[0]) };
            }

            // fitting comment
            return Array.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeAll(INamedTypeSymbol symbol, Compilation compilation)
        {
            var typeIssues = base.AnalyzeType(symbol, compilation);

            if (typeIssues.IsEmptyArray() is false)
            {
                foreach (var typeIssue in typeIssues)
                {
                    yield return typeIssue;
                }
            }

            var methods = symbol.GetNamedMethods();
            var methodsCount = methods.Count;

            if (methodsCount > 0)
            {
                for (var index = 0; index < methodsCount; index++)
                {
                    var methodIssues = AnalyzeMethod(methods[index], compilation);

                    if (methodIssues.IsEmptyArray())
                    {
                        continue;
                    }

                    foreach (var methodIssue in methodIssues)
                    {
                        yield return methodIssue;
                    }
                }
            }
        }
    }
}