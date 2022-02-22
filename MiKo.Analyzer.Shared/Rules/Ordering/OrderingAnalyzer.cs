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

        protected static IEnumerable<IMethodSymbol> GetMethodsOrderedByLocation(INamedTypeSymbol type, MethodKind kind = MethodKind.Ordinary) => GetMethodsOrderedByLocation(type, type.Locations.First(_ => _.IsInSource).GetLineSpan().Path, kind);

        protected static IEnumerable<IMethodSymbol> GetMethodsOrderedByLocation(INamedTypeSymbol type, string path, MethodKind kind = MethodKind.Ordinary) => type.GetMethods(kind)
                                                                                                                                                                  .Where(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().Path == path)
                                                                                                                                                                  .OrderBy(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().StartLinePosition);
    }
}