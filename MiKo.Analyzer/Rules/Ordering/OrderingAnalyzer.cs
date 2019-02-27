using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    public abstract class OrderingAnalyzer : Analyzer
    {
        protected OrderingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(nameof(Ordering), diagnosticId, kind)
        {
        }

        protected static IEnumerable<IMethodSymbol> GetMethodsOrderedByLocation(INamedTypeSymbol type) => GetMethodsOrderedByLocation(type, type.Locations.First(_ => _.IsInSource).GetLineSpan().Path);

        protected static IEnumerable<IMethodSymbol> GetMethodsOrderedByLocation(INamedTypeSymbol type, string path) => type.GetMembers()
                                                                                                                           .OfType<IMethodSymbol>()
                                                                                                                           .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                                                                           .Where(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().Path == path)
                                                                                                                           .OrderBy(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().StartLinePosition);
    }
}