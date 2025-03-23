using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2235_GoingToPhraseAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2235";

        private const string Phrase = "going to";

        public MiKo_2235_GoingToPhraseAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var textTokens = comment.GetXmlTextTokens();
            var textTokensCount = textTokens.Count;

            if (textTokensCount == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> issues = null;

            for (var index = 0; index < textTokensCount; index++)
            {
                var token = textTokens[index];

                var allLocations = GetAllLocations(token, Phrase, StringComparison.OrdinalIgnoreCase);
                var allLocationsCount = allLocations.Count;

                if (allLocationsCount > 0)
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(allLocationsCount);
                    }

                    for (var locationIndex = 0; locationIndex < allLocationsCount; locationIndex++)
                    {
                        issues.Add(Issue(allLocations[locationIndex]));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}