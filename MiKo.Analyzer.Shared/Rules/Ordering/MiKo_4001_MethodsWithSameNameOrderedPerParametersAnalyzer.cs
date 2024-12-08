﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4001";

        public MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            var ctors = GetMethodsOrderedByLocation(symbol, MethodKind.Constructor);
            var methods = GetMethodsOrderedByLocation(symbol);
            var methodsAndCtors = ctors.Concat(methods).ToList();

            return methodsAndCtors.Count != 0
                   ? AnalyzeMethods(methodsAndCtors)
                   : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeMethods(IEnumerable<IMethodSymbol> methods)
        {
            var methodNames = methods.ToHashSet(_ => _.Name);

            foreach (var methodName in methodNames)
            {
                // pre-order for accessibility (public first, private last), then ensure that static methods are first and params methods are at the end
                var methodsOrderedByParameters = Orderer.GetMethodsOrderedByParameters(methods, methodName);

                if (methodsOrderedByParameters.Count <= 1)
                {
                    continue;
                }

                // pre-order for accessibility (public first, private last), then ensure that static methods are first and params methods are at the end
                foreach (var similarMethods in methodsOrderedByParameters.GroupBy(_ => _.DeclaredAccessibility))
                {
                    if (similarMethods.Count() <= 1)
                    {
                        continue;
                    }

                    var builder = StringBuilderCache.Acquire();

                    foreach (var similarMethod in similarMethods)
                    {
                        similarMethod.GetMethodSignature(builder.Append("   ")).AppendLine();
                    }

                    var order = StringBuilderCache.GetStringAndRelease(builder);

                    // check for locations
                    var lastLine = similarMethods.First().GetStartingLine();

                    foreach (var method in similarMethods)
                    {
                        var nextLine = method.GetStartingLine();

                        if (lastLine > nextLine)
                        {
                            yield return Issue(method, order);
                        }

                        lastLine = nextLine;
                    }
                }
            }
        }
    }
}