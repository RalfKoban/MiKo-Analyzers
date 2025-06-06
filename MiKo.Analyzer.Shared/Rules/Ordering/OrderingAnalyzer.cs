﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    public abstract class OrderingAnalyzer : Analyzer
    {
        protected OrderingAnalyzer(string diagnosticId, in SymbolKind kind = SymbolKind.NamedType) : base(nameof(Ordering), diagnosticId, kind)
        {
        }

        protected static IReadOnlyList<IMethodSymbol> GetMethodsOrderedByLocation(INamedTypeSymbol type, MethodKind kind = MethodKind.Ordinary) => GetMethodsOrderedByLocation(type, type.Locations.First(_ => _.IsInSource).GetLineSpan().Path, kind);

        protected static IReadOnlyList<IMethodSymbol> GetMethodsOrderedByLocation(INamedTypeSymbol type, string path, MethodKind kind = MethodKind.Ordinary) => type.GetMethods(kind)
                                                                                                                                                                    .Where(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().Path == path)
                                                                                                                                                                    .OrderBy(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().StartLinePosition)
                                                                                                                                                                    .ToList();

        protected static IReadOnlyList<IFieldSymbol> GetFieldsOrderedByLocation(INamedTypeSymbol type, string path) => type.GetFields()
                                                                                                                           .Where(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().Path == path)
                                                                                                                           .OrderBy(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().StartLinePosition)
                                                                                                                           .ToList();
    }
}