using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1099_ParametersOnOverloadsNameSchemeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1099";

        public MiKo_1099_ParametersOnOverloadsNameSchemeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
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

        private static void FilterIdenticalParameters(List<IParameterSymbol> referenceParameters, List<IParameterSymbol> otherParameters)
        {
            var otherIndex = 0;
            var referenceIndex = 0;

            var indices = new Stack<int>();

            // filter all parameters that are the same on the same position
            while (otherIndex < otherParameters.Count && referenceIndex < referenceParameters.Count)
            {
                var otherParameter = otherParameters[otherIndex];
                var referenceParameter = referenceParameters[referenceIndex];

                if (otherParameter.Name == referenceParameter.Name && otherParameter.Type.Equals(referenceParameter.Type, SymbolEqualityComparer.Default))
                {
                    // remove this one
                    indices.Push(referenceIndex);
                }
                else
                {
                    break;
                }

                otherIndex++;
                referenceIndex++;
            }

            while (indices.Count > 0)
            {
                var index = indices.Pop();

                referenceParameters.RemoveAt(index);
                otherParameters.RemoveAt(index);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeMethods(List<IMethodSymbol> methods)
        {
            var referenceMethod = methods[0];

            foreach (var otherMethod in methods.Skip(1)) // skip the longest method as that one acts as reference
            {
                var referenceParameters = referenceMethod.Parameters.ToList();
                var otherParameters = otherMethod.Parameters.ToList();

                if (referenceParameters.Count == otherParameters.Count)
                {
                    referenceParameters.Reverse();
                    otherParameters.Reverse();

                    FilterIdenticalParameters(referenceParameters, otherParameters);

                    referenceParameters.Reverse();
                    otherParameters.Reverse();
                }

                FilterIdenticalParameters(referenceParameters, otherParameters);

                var referenceIndex = 0;
                var otherIndex = 0;

                while (otherIndex < otherParameters.Count && referenceIndex < referenceParameters.Count)
                {
                    var otherParameter = otherParameters[otherIndex];
                    var referenceParameter = referenceParameters[referenceIndex];

                    var otherParameterName = otherParameter.Name;
                    var referenceParameterName = referenceParameter.Name;

                    referenceIndex++;
                    otherIndex++;

                    if (otherParameterName == referenceParameterName
                     || otherParameterName.Equals(referenceParameterName.AsSpan().WithoutNumberSuffix(), StringComparison.Ordinal))
                    {
                        continue;
                    }

                    // names do not match
                    if (otherParameter.Type.Equals(referenceParameter.Type, SymbolEqualityComparer.Default))
                    {
                        // types are equal, so renamed
                        yield return Issue(otherParameter, referenceParameterName, CreateBetterNameProposal(referenceParameterName));
                    }
                    else
                    {
                        // type and name do not match, so it may be a different parameter, hence skip the current reference parameter but do a re-check for the parameter
                        otherIndex--;
                    }
                }
            }
        }
    }
}