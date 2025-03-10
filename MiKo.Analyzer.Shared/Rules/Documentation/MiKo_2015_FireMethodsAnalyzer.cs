using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2015_FireMethodsAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2015";

        private static readonly string[] AllowedWords = { "raise", "throw" };
        private static readonly string[] ForbiddenWords = { "fire", "fired", "fires", "firing" };

        private static readonly string AllowedWordsForRule = AllowedWords.HumanizedConcatenated();
        private static readonly string ForbiddenWordsForRule = ForbiddenWords.HumanizedConcatenated();

        private static readonly string[] ForbiddenPhrases = ForbiddenWords.WithDelimiters();

        public MiKo_2015_FireMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                case SymbolKind.Method:
                case SymbolKind.Property:
                case SymbolKind.Event:
                case SymbolKind.Field:
                    return true;

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var textTokens = comment.GetXmlTextTokens();
            var textTokensCount = textTokens.Count;

            if (textTokensCount == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var text = textTokens.GetTextTrimmedWithParaTags();

            if (text.ContainsAny(ForbiddenWords, StringComparison.OrdinalIgnoreCase) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> issues = null;

            for (var i = 0; i < textTokensCount; i++)
            {
                var locations = GetAllLocations(textTokens[i], ForbiddenPhrases, StringComparison.OrdinalIgnoreCase);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(locationsCount);
                    }

                    for (var index = 0; index < locationsCount; index++)
                    {
                        issues.Add(Issue(symbol.Name, locations[index], AllowedWordsForRule, ForbiddenWordsForRule));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}