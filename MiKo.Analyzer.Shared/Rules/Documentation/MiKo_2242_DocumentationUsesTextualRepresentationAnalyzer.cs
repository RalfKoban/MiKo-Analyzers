using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2242_DocumentationUsesTextualRepresentationAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2242";

        private const string Phrase = "string representation";

        public MiKo_2242_DocumentationUsesTextualRepresentationAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> issues = null;

            foreach (var xmlText in comment.DescendantNodes<XmlTextSyntax>())
            {
                var textTokens = xmlText.TextTokens;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var textTokensCount = textTokens.Count;

                if (textTokensCount is 0)
                {
                    continue;
                }

                var parent = xmlText.Parent;

                if (parent.IsCode())
                {
                    continue;
                }

                for (var index = 0; index < textTokensCount; index++)
                {
                    var token = textTokens[index];

                    if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                    {
                        continue;
                    }

                    var locations = GetAllLocations(token, Phrase, StringComparison.OrdinalIgnoreCase);

                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(locations.Count);
                    }

                    foreach (var location in locations)
                    {
                        issues.Add(Issue(location));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}