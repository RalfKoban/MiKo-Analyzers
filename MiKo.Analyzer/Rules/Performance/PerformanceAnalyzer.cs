﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    public abstract class PerformanceAnalyzer : Analyzer
    {
        protected PerformanceAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(nameof(Performance), diagnosticId, kind)
        {
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method) => ShallAnalyze(method)
                                                                                                     ? Analyze(method)
                                                                                                     : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol field) => ShallAnalyze(field)
                                                                                                  ? Analyze(field)
                                                                                                  : Enumerable.Empty<Diagnostic>();

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => !symbol.IsOverride;

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => !symbol.IsOverride;

        protected virtual IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol) => Enumerable.Empty<Diagnostic>();
    }

}