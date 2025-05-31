using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2208";

        private static readonly string[] Phrases = Constants.Comments.InstanceOfPhrases;

        private static readonly int MinimumPhraseLength = Phrases.Min(_ => _.Length);

        public MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer() : base(Id)
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

            var text = textTokens.GetTextTrimmedWithParaTags();

            if (text.ContainsAny(Phrases, StringComparison.Ordinal) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;

            for (var i = 0; i < textTokensCount; i++)
            {
                var token = textTokens[i];

                if (token.ValueText.Length < MinimumPhraseLength)
                {
                    continue;
                }

                const int EndOffset = 1; // we do not want to underline the last char

                var locations = GetAllLocations(token, Phrases, StringComparison.Ordinal, 0, EndOffset);
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