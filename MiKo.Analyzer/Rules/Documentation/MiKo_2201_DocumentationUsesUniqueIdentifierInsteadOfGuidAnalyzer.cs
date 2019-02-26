﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2201_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2201";

        private static readonly string[] Guids = new[] { "guid", " Guid" }.SelectMany(_ => Constants.Comments.Delimiters, (_, delimiter) => " " + _ + delimiter).ToArray();

        public MiKo_2201_DocumentationUsesUniqueIdentifierInsteadOfGuidAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => commentXml.ContainsAny(Guids, StringComparison.Ordinal)
                                                                                                            ? new[] { ReportIssue(symbol) }
                                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}