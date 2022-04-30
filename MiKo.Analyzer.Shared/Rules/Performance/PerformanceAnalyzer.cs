using System.Collections.Generic;
using System.Linq;

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
                                                                                                                              : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                            ? Analyze(symbol, compilation)
                                                                                                                            : Enumerable.Empty<Diagnostic>();

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsOverride is false;

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => symbol.IsOverride is false;

        protected virtual IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();
    }
}