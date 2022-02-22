using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1001_EventArgsParameterAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1001";

        public MiKo_1001_EventArgsParameterAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IParameterSymbol symbol)
        {
            var method = symbol.GetEnclosingMethod();

            var applicableParameters = method.Parameters.Where(_ => IsApplicable(_.Type)).ToList();
            if (applicableParameters.Count == 1)
            {
                return method.Name == nameof(Equals) ? "other" : "e";
            }

            var i = applicableParameters.IndexOf(symbol);

            return "e" + i;
        }

        internal static bool IsAccepted(IParameterSymbol parameter)
        {
            var method = parameter.GetEnclosingMethod();

            return GetParameters(method).Contains(parameter) && FindBetterName(parameter) == parameter.Name;
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.IsOverride)
            {
                return false;
            }

            if (symbol.MethodKind == MethodKind.PropertySet)
            {
                return false; // ignore the setter as the name there has to be 'value'
            }

            if (symbol.IsEventHandler())
            {
                return false; // ignore the method as it is handled by MiKo_1002_EventHandlingMethodParametersAnalyzer
            }

            if (symbol.IsDependencyObjectEventHandler())
            {
                return false; // ignore the method as it is handled by MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer
            }

            if (symbol.IsInterfaceImplementation())
            {
                return false; // keep names as in interface
            }

            return true;
        }

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var parameters = GetParameters(symbol);

            var count = parameters.Count;

            if (count > 0)
            {
                var i = 0;

                foreach (var parameter in parameters)
                {
                    var expected = count == 1
                                       ? (symbol.Name == nameof(Equals) ? "other" : "e")
                                       : "e" + i;
                    i++;

                    if (parameter.Name != expected)
                    {
                        yield return Issue(parameter, expected);
                    }
                }
            }
        }

        private static bool IsApplicable(ITypeSymbol type) => type.IsEventArgs() || type.IsDependencyPropertyChangedEventArgs();

        private static List<IParameterSymbol> GetParameters(IMethodSymbol method)
        {
            var parameters = method.Parameters.Where(_ => IsApplicable(_.Type)).ToList();

            return parameters;
        }
    }
}