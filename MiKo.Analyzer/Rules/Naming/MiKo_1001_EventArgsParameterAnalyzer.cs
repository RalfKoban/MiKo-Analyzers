﻿using System.Collections.Generic;
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

            var applicableParameters = method.Parameters.Where(_ => _.Type.IsEventArgs()).ToList();
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

            if (symbol.IsDependencyPropertyEventHandler())
            {
                return false; // ignore the method as it is handled by MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer
            }

            if (symbol.IsInterfaceImplementation())
            {
                return false; // keep names as in interface
            }

            return true;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            var parameters = GetParameters(symbol);
            switch (parameters.Count)
            {
                case 0: return Enumerable.Empty<Diagnostic>();
                case 1:
                    {
                        var expected = symbol.Name != nameof(Equals) ? "e" : "other";

                        var parameter = parameters[0];

                        return parameter.Name != expected
                                   ? new[] { Issue(parameter, expected) }
                                   : Enumerable.Empty<Diagnostic>();
                    }

                default:
                    {
                        var i = 0;
                        var diagnostics = new List<Diagnostic>(parameters.Count);
                        foreach (var parameter in parameters)
                        {
                            var expected = "e" + i;

                            if (parameter.Name != expected)
                            {
                                diagnostics.Add(Issue(parameter, expected));
                            }

                            i++;
                        }

                        return diagnostics;
                    }
            }
        }

        private static List<IParameterSymbol> GetParameters(IMethodSymbol method)
        {
            var parameters = method.Parameters.Where(_ => _.Type.IsEventArgs() || _.Type.IsDependencyPropertyChangedEventArgs()).ToList();

            return parameters;
        }
    }
}