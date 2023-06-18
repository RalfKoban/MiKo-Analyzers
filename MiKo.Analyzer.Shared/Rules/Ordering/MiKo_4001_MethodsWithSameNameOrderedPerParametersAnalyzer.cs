using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4001";

        public MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        internal static List<IMethodSymbol> GetMethodsOrderedByParameters(IEnumerable<IMethodSymbol> methods, string methodName) => methods.Where(_ => _.Name == methodName)
                                                                                                                                           .OrderByDescending(_ => _.DeclaredAccessibility)
                                                                                                                                           .ThenByDescending(_ => _.IsStatic)
                                                                                                                                           .ThenBy(_ => _.Parameters.Any(__ => __.IsParams))
                                                                                                                                           .ThenBy(_ => _.Parameters.Length)
                                                                                                                                           .ToList();

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            var ctors = GetMethodsOrderedByLocation(symbol, MethodKind.Constructor);
            var methods = GetMethodsOrderedByLocation(symbol);
            var methodsAndCtors = ctors.Concat(methods).ToList();

            return methodsAndCtors.Any()
                   ? AnalyzeMethods(methodsAndCtors)
                   : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeMethods(IEnumerable<IMethodSymbol> methods)
        {
            var methodNames = methods.ToHashSet(_ => _.Name);

            foreach (var methodName in methodNames)
            {
                // pre-order for accessibility (public first, private last), then ensure that static methods are first and params methods are at the end
                var methodsOrderedByParameters = GetMethodsOrderedByParameters(methods, methodName);

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

                    var order = similarMethods.Select(_ => "   " + _.GetMethodSignature()).ConcatenatedWith(Constants.EnvironmentNewLine);

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