﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2214_DocumentationContainsEmptyLinesAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2214";

        public MiKo_2214_DocumentationContainsEmptyLinesAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
        {
            List<Diagnostic> results = null;

            foreach (var tokens in comment.DescendantNodes<XmlTextSyntax>().Select(_ => _.TextTokens))
            {
                var count = tokens.Count - 1;

                if (count > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var currentToken = tokens[i];

                        if (currentToken.IsKind(SyntaxKind.XmlTextLiteralToken) && currentToken.LeadingTrivia.Any(SyntaxKind.DocumentationCommentExteriorTrivia) && currentToken.ValueText.IsNullOrWhiteSpace())
                        {
                            var nextToken = tokens[i + 1];

                            if (nextToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                            {
                                if (results is null)
                                {
                                    results = new List<Diagnostic>(1);
                                }

                                results.Add(Issue(nextToken));
                            }
                        }
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}