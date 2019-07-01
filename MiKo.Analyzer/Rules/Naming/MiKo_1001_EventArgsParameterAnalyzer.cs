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

        protected override bool ShallAnalyze(IMethodSymbol method)
        {
            if (method.IsOverride)
            {
                return false;
            }

            if (method.MethodKind == MethodKind.PropertySet)
            {
                return false; // ignore the setter as the name there has to be 'value'
            }

            if (method.IsEventHandler())
            {
                return false; // ignore the method as it is handled by MiKo_1002_EventHandlingMethodParametersAnalyzer
            }

            if (method.IsDependencyPropertyEventHandler())
            {
                return false; // ignore the method as it is handled by MiKo_1007_DependencyPropertyEventHandlingMethodParametersAnalyzer
            }

            return true;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var parameters = method.Parameters.Where(_ => _.Type.IsEventArgs() || _.Type.IsDependencyPropertyChangedEventArgs()).ToList();
            switch (parameters.Count)
            {
                case 0: return Enumerable.Empty<Diagnostic>();
                case 1:
                    {
                        var expected = method.Name != nameof(Equals) ? "e" : "other";

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
                            var parameterName = parameter.Name;

                            var expected = "e" + (++i);
                            if (parameterName != expected)
                            {
                                diagnostics.Add(Issue(parameter, expected));
                            }
                        }

                        return diagnostics;
                    }
            }
        }
    }
}