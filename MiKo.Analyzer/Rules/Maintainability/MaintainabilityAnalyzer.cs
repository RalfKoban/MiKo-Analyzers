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

        protected sealed override IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                    ? Analyze(symbol, compilation)
                                                                                                                                    : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                               ? Analyze(symbol, compilation)
                                                                                                                               : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                              ? Analyze(symbol, compilation)
                                                                                                                              : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                  ? Analyze(symbol, compilation)
                                                                                                                                  : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                            ? Analyze(symbol, compilation)
                                                                                                                            : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                            ? Analyze(symbol, compilation)
                                                                                                                            : Enumerable.Empty<Diagnostic>();

        protected virtual bool ShallAnalyze(INamespaceSymbol symbol) => true;

        protected virtual bool ShallAnalyze(INamedTypeSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsOverride is false;

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => symbol.IsOverride is false;

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => symbol.IsOverride is false;

        protected virtual bool ShallAnalyze(IEventSymbol symbol) => symbol.IsOverride is false;

        protected virtual IEnumerable<Diagnostic> Analyze(INamespaceSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> Analyze(IEventSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();
    }
}