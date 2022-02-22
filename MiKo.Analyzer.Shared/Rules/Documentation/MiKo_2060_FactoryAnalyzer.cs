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

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsFactory(); // do not call base.ShallAnalyze() here to avoid that we don't inspect the methods of the type

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.MethodKind == MethodKind.Ordinary && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            // let's see if the type contains a documentation XML
            var typeIssues = comment != null && base.ShallAnalyze(symbol)
                                 ? base.AnalyzeType(symbol, compilation, comment)
                                 : Enumerable.Empty<Diagnostic>();

            return typeIssues.Concat(symbol.GetMethods().SelectMany(_ => AnalyzeMethod(_, compilation)));
        }

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node)
        {
            switch (symbol)
            {
                case INamedTypeSymbol _: return Issue(symbol.Name, node, Constants.Comments.FactorySummaryPhrase);
                case IMethodSymbol _: return Issue(symbol.Name, node, Phrase);
                default: return null;
            }
        }

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;

            switch (symbol)
            {
                case INamedTypeSymbol _:
                    {
                        if (summary.StartsWith(Constants.Comments.FactorySummaryPhrase, StringComparison.Ordinal))
                        {
                            return null;
                        }

                        var firstWord = summary.FirstWord();
                        var location = GetFirstLocation(textToken, firstWord);

                        return Issue(symbol.Name, location, Constants.Comments.FactorySummaryPhrase);
                    }

                case IMethodSymbol method:
                    {
                        var phrases = GetPhrases(method).ToArray();

                        if (summary.StartsWithAny(phrases, StringComparison.Ordinal))
                        {
                            return null;
                        }

                        var firstWord = summary.FirstWord();
                        var location = GetFirstLocation(textToken, firstWord);

                        return Issue(symbol.Name, location, phrases.First());
                    }

                default:
                    return null;
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
            if (comments.Any(_ => phrases.Any(__ => _.StartsWith(__, StringComparison.Ordinal))))
            {
                // fitting comment
            }
            else
            {
                yield return Issue(symbol, phrases.First());
            }
        }
    }
}