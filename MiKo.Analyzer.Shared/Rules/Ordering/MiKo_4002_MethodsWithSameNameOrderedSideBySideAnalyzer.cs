﻿using System;
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

        public MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            var ctors = GetMethodsOrderedByLocation(symbol, MethodKind.Constructor);

            if (symbol.IsRecord)
            {
                // filter primary ctors (as we cannot re-align those)
                ctors = ctors.Where(_ => _.IsPrimaryConstructor() is false).ToList();
            }

            var methods = GetMethodsOrderedByLocation(symbol);

            var count = ctors.Count + methods.Count;

            if (count <= 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var methodsAndCtors = new List<IMethodSymbol>(count);
            methodsAndCtors.AddRange(ctors);
            methodsAndCtors.AddRange(methods);

            return AnalyzeMethodsGroupedByAccessibility(methodsAndCtors);
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
                // we have a more than 1 method with same name, so we have to detect the index, sort the index and then see if the indices differ by more than one
                var indices = new HashSet<int>(methodsWithSameName.Select(allMethods.IndexOf));

                var startIndex = indices.First();

                foreach (var index in indices)
                {
                    if (Math.Abs(index - startIndex) > 1)
                    {
                        var method = allMethods[index];

                        var builder = StringBuilderCache.Acquire();

                        foreach (var other in methodsWithSameName)
                        {
                            if (ReferenceEquals(method, other) is false)
                            {
                                var sb = builder.Append("   ");

                                other.GetMethodSignature(sb);

                                sb.AppendLine();
                            }
                        }

                        var signatures = StringBuilderCache.GetStringAndRelease(builder);

                        yield return Issue(method, signatures);
                    }

                    startIndex = index;
                }
            }
        }
    }
}