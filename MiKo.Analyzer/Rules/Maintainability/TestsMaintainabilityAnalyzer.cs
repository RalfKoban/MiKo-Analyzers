using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class TestsMaintainabilityAnalyzer : MaintainabilityAnalyzer
    {
        protected TestsMaintainabilityAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.NamedType) : base(diagnosticId, kind)
        {
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.IsTestClass()
                                                                                                      ? AnalyzeTestType(symbol)
                                                                                                      : Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeTestType(INamedTypeSymbol symbol) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<IMethodSymbol> GetTestMethods(INamedTypeSymbol symbol) => symbol.IncludingAllBaseTypes()
                                                                                              .SelectMany(_ => _.GetMembers().OfType<IMethodSymbol>())
                                                                                              .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                                              .Where(_ => _.IsTestMethod());
    }
}