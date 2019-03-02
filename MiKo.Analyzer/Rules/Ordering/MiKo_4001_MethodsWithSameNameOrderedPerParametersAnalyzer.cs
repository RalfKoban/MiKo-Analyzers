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
                // pre-order for accessibility (public first, private last)
                var methodsOrderedByParameters = methods.Where(_ => _.Name == methodName).OrderByDescending(_ => _.DeclaredAccessibility).ThenBy(_ => _.Parameters.Length).ToList();
                if (methodsOrderedByParameters.Count <= 1)
                    continue;

                var order = string.Empty;

                // check for locations
                var lastLine = GetStartingLine(methodsOrderedByParameters[0]);

                foreach (var method in methodsOrderedByParameters)
                {
                    var nextLine = GetStartingLine(method);
                    if (lastLine > nextLine)
                    {
                        if (results == null)
                        {
                            results = new List<Diagnostic>();
                            order = string.Join(Environment.NewLine, methodsOrderedByParameters.Select(_ => "   " + _.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
                        }

                        results.Add(ReportIssue(method, order));
                    }

                    lastLine = nextLine;
                }
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }

        private static int GetStartingLine(IMethodSymbol method) => method.Locations.First(__ => __.IsInSource).GetLineSpan().StartLinePosition.Line;
    }
}