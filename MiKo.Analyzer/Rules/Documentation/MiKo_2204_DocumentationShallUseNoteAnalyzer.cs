﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2204_DocumentationShallUseNoteAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2204";

        private static readonly string[] Triggers = { "Attention", "Caution!", "Caution:", "Caution !", "  Important: ", "  Note: ", "  Please note: " };

        public MiKo_2204_DocumentationShallUseNoteAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => commentXml.ContainsAny(Triggers, StringComparison.OrdinalIgnoreCase)
                                                                                                            ? new[] { ReportIssue(symbol) }
                                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}