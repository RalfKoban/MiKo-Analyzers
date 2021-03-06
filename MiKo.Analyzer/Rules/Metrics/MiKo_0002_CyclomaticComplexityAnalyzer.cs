﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_0002_CyclomaticComplexityAnalyzer : MetricsAnalyzer
    {
        public const string Id = "MiKo_0002";

        public MiKo_0002_CyclomaticComplexityAnalyzer() : base(Id)
        {
        }

        public int MaxCyclomaticComplexity { get; set; } = 7;

        protected override Diagnostic AnalyzeBody(BlockSyntax body, ISymbol owningSymbol)
        {
            var cc = Counter.CountCyclomaticComplexity(body);
            TryCreateDiagnostic(owningSymbol, cc, MaxCyclomaticComplexity, out var diagnostic);

            return diagnostic;
        }
    }
}