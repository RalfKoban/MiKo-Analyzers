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
            var issues = symbol.GetNamedMethods()
                               .Where(ShallAnalyze)
                               .GroupBy(_ => _.Name)
                               .Where(_ => _.MoreThan(1)) // we have more than 1 method with the name, so inspect the parameters
                               .SelectMany(AnalyzeMethods)
                               .ToArray();

            return issues;
        }

        private static void RemoveIdenticalParameters(List<IParameterSymbol> referenceParameters, List<IParameterSymbol> otherParameters)
        {
            RemoveIdenticalParameters(referenceParameters, otherParameters, (r, o) => true);
        }

        private static void RemoveIdenticalParameters(List<IParameterSymbol> referenceParameters, List<IParameterSymbol> otherParameters, Func<IParameterSymbol, IParameterSymbol, bool> comparer)
        {
            var otherIndex = 0;
            var referenceIndex = 0;

            var indices = new Stack<int>();

            // filter all parameters that are the same on the same position
            while (otherIndex < otherParameters.Count && referenceIndex < referenceParameters.Count)
            {
                var otherParameter = otherParameters[otherIndex];
                var referenceParameter = referenceParameters[referenceIndex];

                if (otherParameter.Name == referenceParameter.Name
                 && otherParameter.Type.Equals(referenceParameter.Type, SymbolEqualityComparer.Default)
                 && comparer(referenceParameter, otherParameter))
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

        private IEnumerable<Diagnostic> AnalyzeMethods(IEnumerable<IMethodSymbol> methodsWithSameName)
        {
            // order by number of parameters, so that we get the one with the most parameters as first one (acts as reference)
            var methods = methodsWithSameName.OrderByDescending(_ => _.Parameters.Length).ToList();

            var referenceMethod = methods[0];

            foreach (var otherMethod in methods.Skip(1)) // skip the longest method as that one acts as reference
            {
                var referenceParameters = referenceMethod.Parameters.ToList();
                var otherParameters = otherMethod.Parameters.ToList();

                if (referenceParameters.Count == otherParameters.Count)
                {
                    // strip shared trailing parameters first to avoid skewing the forward comparison
                    referenceParameters.Reverse();
                    otherParameters.Reverse();

                    RemoveIdenticalParameters(referenceParameters, otherParameters);

                    // change back to original order
                    referenceParameters.Reverse();
                    otherParameters.Reverse();
                }

                RemoveIdenticalParameters(referenceParameters, otherParameters);

                // strip shared trailing out-parameters (e.g. 'out int result') before forward traversal
                referenceParameters.Reverse();
                otherParameters.Reverse();

                RemoveIdenticalParameters(referenceParameters, otherParameters, (r, o) => r.IsOut() && o.IsOut());

                // change back to original order
                referenceParameters.Reverse();
                otherParameters.Reverse();

                var referenceIndex = 0;
                var otherIndex = 0;

                // now compare the remaining parameters
                while (otherIndex < otherParameters.Count && referenceIndex < referenceParameters.Count)
                {
                    var otherParameter = otherParameters[otherIndex];
                    var referenceParameter = referenceParameters[referenceIndex];

                    var otherParameterName = otherParameter.Name;
                    var referenceParameterName = referenceParameter.Name;

                    referenceIndex++;
                    otherIndex++;

                    if (otherParameterName == referenceParameterName
                     || otherParameterName.Equals(referenceParameterName.AsSpan().WithoutNumberSuffix()))
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