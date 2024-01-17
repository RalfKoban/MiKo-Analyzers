using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4006_ParametersOrderedInOverloadsAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4006";

        public MiKo_4006_ParametersOrderedInOverloadsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            foreach (var methods in symbol.GetNamedMethods().Where(ShallAnalyze).GroupBy(_ => _.Name))
            {
                if (methods.MoreThan(1))
                {
                    // order by number of parameters, so that we get the one with the most parameters as first one (acts as reference)
                    var methodsWithSameName = methods.OrderByDescending(_ => _.Parameters.Length).ToList();

                    // we have more than 1 method with the name, so inspect the parameters
                    foreach (var diagnostic in AnalyzeMethods(methodsWithSameName))
                    {
                        yield return diagnostic;
                    }
                }
            }
        }

        private static bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.IsOverride)
            {
                return false;
            }

            switch (symbol.MethodKind)
            {
                case MethodKind.Ordinary:
                    return symbol.IsInterfaceImplementation() is false;

                case MethodKind.LocalFunction:
                    return true;

                default:
                    return false;
            }
        }

        private IEnumerable<Diagnostic> AnalyzeMethods(IReadOnlyList<IMethodSymbol> methods)
        {
            var referenceMethod = methods[0];

            var parameterNames = referenceMethod.Parameters.Select(_ => _.Name).ToList();

            foreach (var otherMethod in methods.Skip(1)) // skip the longest method as that one acts as reference
            {
                var otherParameterNames = otherMethod.Parameters.Select(_ => _.Name).ToList();

                var referenceIdenticalParameters = parameterNames.Intersect(otherParameterNames).ToList();
                var otherIdenticalParameters = otherParameterNames.Intersect(parameterNames).ToList();

                if (referenceIdenticalParameters.SequenceEqual(otherIdenticalParameters))
                {
                    continue;
                }

                // TODO RKN: detect the re-ordered parameters, analyze per parameter
                yield return Issue(otherMethod);
            }
        }
    }
}