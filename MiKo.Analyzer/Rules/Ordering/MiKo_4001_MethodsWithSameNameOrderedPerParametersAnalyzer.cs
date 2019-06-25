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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
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
            var methodNames = methods.Select(_ => _.Name).ToHashSet();

            List<Diagnostic> results = null;
            foreach (var methodName in methodNames)
            {
                // pre-order for accessibility (public first, private last), then ensure that static methods are first and params methods are at the end
                var methodsOrderedByParameters = methods.Where(_ => _.Name == methodName)
                                                        .OrderByDescending(_ => _.DeclaredAccessibility)
                                                        .ThenByDescending(_ => _.IsStatic)
                                                        .ThenBy(_ => _.Parameters.Any(__ => __.IsParams))
                                                        .ThenBy(_ => _.Parameters.Length)
                                                        .ToList();
                if (methodsOrderedByParameters.Count <= 1)
                    continue;

                var order = methodsOrderedByParameters.Select(_ => "   " + _.GetMethodSignature()).ConcatenatedWith(Environment.NewLine);

                // check for locations
                var lastLine = methodsOrderedByParameters[0].GetStartingLine();

                foreach (var method in methodsOrderedByParameters)
                {
                    var nextLine = method.GetStartingLine();
                    if (lastLine > nextLine)
                    {
                        if (results is null)
                            results = new List<Diagnostic>();

                        results.Add(Issue(method, order));
                    }

                    lastLine = nextLine;
                }
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}