﻿using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                foreach (var location in GetAllLocations(token, Constants.Comments.WasNotSuccessfulPhrase))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}