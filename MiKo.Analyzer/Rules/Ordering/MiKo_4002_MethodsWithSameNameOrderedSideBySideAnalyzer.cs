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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            var ctors = GetMethodsOrderedByLocation(symbol, MethodKind.Constructor);
            var methods = GetMethodsOrderedByLocation(symbol);
            var methodsAndCtors = ctors.Concat(methods).ToList();

            return methodsAndCtors.Any()
                       ? AnalyzeMethodsGroupedByAccessibility(methodsAndCtors)
                       : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeMethodsGroupedByAccessibility(IEnumerable<IMethodSymbol> methods) => methods.GroupBy(_ => _.DeclaredAccessibility)
                                                                                                                           .Where(_ => _.MoreThan(1))
                                                                                                                           .SelectMany(AnalyzeMethodsGroupedByStatic);

        private IEnumerable<Diagnostic> AnalyzeMethodsGroupedByStatic(IEnumerable<IMethodSymbol> methods) => methods.GroupBy(_ => _.IsStatic)
                                                                                                                    .Where(_ => _.MoreThan(1))
                                                                                                                    .SelectMany(AnalyzeMethods);

        private IEnumerable<Diagnostic> AnalyzeMethods(IEnumerable<IMethodSymbol> methods)
        {
            var orderedMethods = methods.OrderBy(_ => _.GetStartingLine()).ToList();

            foreach (var methodsWithSameName in orderedMethods.GroupBy(_ => _.Name).Where(_ => _.MoreThan(1)))
            {
                // we have a more than 1 method with same name, so we have do detect the index, sort the index and then see if the indices differ by more than one
                var indices = new HashSet<int>(methodsWithSameName.Select(_ => orderedMethods.IndexOf(_)));

                var startIndex = indices.First();
                foreach (var index in indices)
                {
                    if (Math.Abs(index - startIndex) > 1)
                    {
                        var method = orderedMethods[index];

                        var signatures = methodsWithSameName.Except(new[] { method })
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