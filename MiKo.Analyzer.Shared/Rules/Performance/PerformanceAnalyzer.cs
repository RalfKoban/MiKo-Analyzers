﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    public abstract class PerformanceAnalyzer : Analyzer
    {
        protected PerformanceAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(nameof(Performance), diagnosticId, kind)
        {
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                          ? Analyze(symbol, compilation)
                                                                                                                          : Array.Empty<Diagnostic>();

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => true;

        protected virtual IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();
    }
}