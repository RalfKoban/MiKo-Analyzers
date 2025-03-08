using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2229_XmlFragmentAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2229";

        private static readonly string[] Phrases = { "</", "/>", "/ >" };

        public MiKo_2229_XmlFragmentAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            List<Diagnostic> results = null;

            foreach (var token in comment.DescendantTokens(SyntaxKind.XmlTextLiteralToken))
            {
                if (token.Parent.IsKind(SyntaxKind.XmlCDataSection))
                {
                    // ignore the code sections
                    continue;
                }

                var locations = GetAllLocations(token, Phrases);
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