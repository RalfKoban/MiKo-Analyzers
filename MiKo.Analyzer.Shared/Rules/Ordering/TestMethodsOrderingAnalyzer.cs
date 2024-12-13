using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    public abstract class TestMethodsOrderingAnalyzer : OrderingAnalyzer
    {
        protected TestMethodsOrderingAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected abstract IMethodSymbol GetMethod(INamedTypeSymbol symbol);

        protected abstract int GetExpectedMethodIndex(IEnumerable<IMethodSymbol> methods);

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            var method = GetMethod(symbol);

            return method != null
                   ? AnalyzeTestType(symbol, method)
                   : Array.Empty<Diagnostic>();
        }

        private Diagnostic[] AnalyzeTestType(INamedTypeSymbol symbol, IMethodSymbol method)
        {
            var path = method.Locations.First(_ => _.IsInSource).GetLineSpan().Path;

            var methods = GetMethodsOrderedByLocation(symbol, path).ToList();

            var index = GetExpectedMethodIndex(methods);

            var otherMethod = methods[index];

            return ReferenceEquals(method, otherMethod)
                   ? Array.Empty<Diagnostic>()
                   : new[] { Issue(method) };
        }
    }
}