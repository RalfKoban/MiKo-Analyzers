using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2212";

        public MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var textTokens = comment.GetXmlTextTokens();
            var textTokensCount = textTokens.Count;

            if (textTokensCount is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            const string Phrase = Constants.Comments.WasNotSuccessfulPhrase;

            var text = textTokens.GetTextTrimmedWithParaTags();

            if (text.Contains(Phrase, StringComparison.Ordinal) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;

            for (var i = 0; i < textTokensCount; i++)
            {
                var token = textTokens[i];

                if (token.ValueText.Length < Phrase.Length)
                {
                    continue;
                }

                var locations = GetAllLocations(token, Phrase);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(locationsCount);
                    }

                    for (var index = 0; index < locationsCount; index++)
                    {
                        results.Add(Issue(locations[index]));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}