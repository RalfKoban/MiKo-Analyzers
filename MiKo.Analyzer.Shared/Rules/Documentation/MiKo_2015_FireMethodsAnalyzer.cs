﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2015_FireMethodsAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2015";

        private static readonly string[] AllowedWords = { "raise", "throw" };
        private static readonly string[] ForbiddenWords = { "fire", "fired", "fires", "firing" };

        private static readonly string AllowedWordsForRule = AllowedWords.HumanizedConcatenated();
        private static readonly string ForbiddenWordsForRule = ForbiddenWords.HumanizedConcatenated();

        private static readonly string[] ForbiddenPhrases = CreateForbiddenPhrases(ForbiddenWords).ToArray();

        public MiKo_2015_FireMethodsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<XmlElementSyntax> summaryXmls) => AnalyzeSummaryContains(symbol, summaryXmls, ForbiddenPhrases);

        protected override Diagnostic SummaryContainsIssue(ISymbol symbol, Location location, string phrase) => Issue(symbol.Name, location, AllowedWordsForRule, ForbiddenWordsForRule);

        private static IEnumerable<string> CreateForbiddenPhrases(IEnumerable<string> forbiddenWords) => from suffix in Constants.Comments.Delimiters from forbiddenWord in forbiddenWords select forbiddenWord + suffix;
    }
}