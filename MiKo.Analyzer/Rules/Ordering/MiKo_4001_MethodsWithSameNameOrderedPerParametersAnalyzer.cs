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
            // TODO RKN: split for accessibility
            var methods = GetMethodsOrderedByLocation(symbol).ToList();

            var results = methods.Any()
                          ? AnalyzeMethods(methods)
                          : Enumerable.Empty<Diagnostic>();

            return results;
        }

        private IEnumerable<Diagnostic> AnalyzeMethods(IEnumerable<IMethodSymbol> methods)
        {
            var methodNames = methods.Select(_ => _.Name).ToHashSet();

            List<Diagnostic> results = null;
            foreach (var methodName in methodNames)
            {
                var methodsOrderedByParameters = methods.Where(_ => _.Name == methodName).OrderBy(_ => _.Parameters.Length).ToList();
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
                            order = string.Join(Environment.NewLine, methodsOrderedByParameters.Select(_ => _.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
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