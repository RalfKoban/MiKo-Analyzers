using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    internal static class Orderer
    {
        internal static List<IMethodSymbol> GetMethodsOrderedByParameters(IEnumerable<IMethodSymbol> methods, string methodName) => methods.Where(_ => _.Name == methodName)
                                                                                                                                           .OrderByDescending(_ => _.DeclaredAccessibility)
                                                                                                                                           .ThenByDescending(_ => _.IsStatic)
                                                                                                                                           .ThenBy(_ => _.Parameters.Any(__ => __.IsParams))
                                                                                                                                           .ThenBy(_ => _.Parameters.Length)
                                                                                                                                           .ToList();

        internal static List<IMethodSymbol> GetMethodsOrderedByStatics(IEnumerable<IMethodSymbol> methods) => methods.OrderByDescending(_ => _.DeclaredAccessibility)
                                                                                                                     .ThenByDescending(_ => _.IsStatic)
                                                                                                                     .ToList();

        internal static List<IMethodSymbol> GetMethodsOrderedByStatics(IEnumerable<IMethodSymbol> methods, string methodName) => GetMethodsOrderedByStatics(methods.Where(_ => _.Name == methodName));
    }
}