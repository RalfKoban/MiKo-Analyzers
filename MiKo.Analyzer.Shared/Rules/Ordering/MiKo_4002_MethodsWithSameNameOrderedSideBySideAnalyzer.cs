using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4002";

        public MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        internal static List<IMethodSymbol> GetMethodsOrderedByStatics(IEnumerable<IMethodSymbol> methods) => methods.OrderByDescending(_ => _.DeclaredAccessibility)
                                                                                                                     .ThenByDescending(_ => _.IsStatic)
                                                                                                                     .ToList();

        internal static List<IMethodSymbol> GetMethodsOrderedByStatics(IEnumerable<IMethodSymbol> methods, string methodName) => GetMethodsOrderedByStatics(methods.Where(_ => _.Name == methodName));

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            var ctors = GetMethodsOrderedByLocation(symbol, MethodKind.Constructor);

            if (symbol.IsRecord)
            {
                // filter primary ctors (as we cannot re-align those)
                ctors = ctors.Where(_ => _.IsPrimaryConstructor() is false);
            }

            var methods = GetMethodsOrderedByLocation(symbol);
            var methodsAndCtors = ctors.Concat(methods).ToList();

            return methodsAndCtors.Any()
                       ? AnalyzeMethodsGroupedByAccessibility(methodsAndCtors)
                       : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeMethodsGroupedByAccessibility(IList<IMethodSymbol> allMethods) => allMethods.GroupBy(_ => _.DeclaredAccessibility)
                                                                                                                           .Where(_ => _.MoreThan(1))
                                                                                                                           .SelectMany(_ => AnalyzeMethodsGroupedByStatic(allMethods, _));

        private IEnumerable<Diagnostic> AnalyzeMethodsGroupedByStatic(IList<IMethodSymbol> allMethods, IEnumerable<IMethodSymbol> methods) => methods.GroupBy(_ => _.IsStatic)
                                                                                                                                                     .Where(_ => _.MoreThan(1))
                                                                                                                                                     .SelectMany(_ => AnalyzeMethods(allMethods, _));

        private IEnumerable<Diagnostic> AnalyzeMethods(IList<IMethodSymbol> allMethods, IEnumerable<IMethodSymbol> methods)
        {
            var orderedMethods = methods.OrderBy(_ => _.GetStartingLine()).ToList();

            foreach (var methodsWithSameName in orderedMethods.GroupBy(_ => _.Name).Where(_ => _.MoreThan(1)))
            {
                // we have a more than 1 method with same name, so we have do detect the index, sort the index and then see if the indices differ by more than one
                var indices = new HashSet<int>(methodsWithSameName.Select(allMethods.IndexOf));

                var startIndex = indices.First();

                foreach (var index in indices)
                {
                    if (Math.Abs(index - startIndex) > 1)
                    {
                        var method = allMethods[index];

                        var signatures = methodsWithSameName.Except(method)
                                                            .Select(_ => "   " + _.GetMethodSignature())
                                                            .ConcatenatedWith(Environment.NewLine);

                        yield return Issue(method, signatures);
                    }

                    startIndex = index;
                }
            }
        }
    }
}