﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2206_FlagAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2206";

        private static readonly string[] Phrases = CreatePhrases(" flag", " flags").ToArray();

        public MiKo_2206_FlagAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => from phrase in Phrases where commentXml.Contains(phrase, StringComparison.OrdinalIgnoreCase) select Issue(symbol, phrase);

        private static IEnumerable<string> CreatePhrases(params string[] forbiddenTerms) => from suffix in Constants.Comments.Delimiters from forbiddenTerm in forbiddenTerms select forbiddenTerm + suffix;
    }
}