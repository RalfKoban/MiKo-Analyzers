﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2203";

        internal static readonly string[] Guids = new[] { "guid", " Guid", "GUID" }.SelectMany(_ => Constants.Comments.Delimiters, (_, delimiter) => " " + _ + delimiter).ToArray();

        public MiKo_2203_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                const int Offset = 1; // we do not want to underline the first and last char
                foreach (var location in GetAllLocations(token, Guids, StringComparison.Ordinal, Offset, Offset))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}