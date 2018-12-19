using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityAnalyzer : Analyzer
    {
        protected MaintainabilityAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(nameof(Maintainability), diagnosticId, kind)
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