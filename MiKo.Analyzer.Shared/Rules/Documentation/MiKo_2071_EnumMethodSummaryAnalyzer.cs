﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2071_EnumMethodSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2071";

        private static readonly string[] BooleanPhrases = (from term1 in new[] { " indicating ", " indicates ", " indicate " }
                                                           from term2 in new[] { "whether ", "if " }
                                                           select string.Concat(term1, term2))
                                                          .ToArray();

        public MiKo_2071_EnumMethodSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsEnum() && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.GetReturnType()?.IsEnum() is true && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<XmlElementSyntax> summaryXmls) => AnalyzeSummaryContains(symbol, summaryXmls, BooleanPhrases);
    }
}