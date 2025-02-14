﻿using System.Collections.Generic;

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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
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
                    for (var index = 0; index < locationsCount; index++)
                    {
                        yield return Issue(locations[index]);
                    }
                }
            }
        }
    }
}