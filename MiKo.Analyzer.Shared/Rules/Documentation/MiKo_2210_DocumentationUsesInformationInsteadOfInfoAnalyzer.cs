﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2210";

        public MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer() : base(Id)
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

            if (text.ContainsAny(Constants.Comments.InfoTerms, StringComparison.OrdinalIgnoreCase) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;

            for (var i = 0; i < textTokensCount; i++)
            {
                const int Offset = 1; // we do not want to underline the first and last char

                var locations = GetAllLocations(textTokens[i], Constants.Comments.InfoTerms, StringComparison.OrdinalIgnoreCase, Offset, Offset);
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