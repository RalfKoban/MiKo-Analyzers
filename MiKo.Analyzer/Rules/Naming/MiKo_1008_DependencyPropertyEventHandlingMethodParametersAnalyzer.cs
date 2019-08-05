using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1008";

        public MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => method.IsDependencyPropertyEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var parameters = method.Parameters;

            List<Diagnostic> diagnostics = null;
            VerifyParameterName("d", parameters[0], ref diagnostics);
            VerifyParameterName("e", parameters[1], ref diagnostics);
            return diagnostics ?? Enumerable.Empty<Diagnostic>();
        }

        private void VerifyParameterName(string expected, IParameterSymbol parameter, ref List<Diagnostic> results)
        {
            if (expected == parameter.Name)
            {
                return;
            }

            if (results is null)
            {
                results = new List<Diagnostic>(1);
            }

            results.Add(Issue(parameter, expected));
        }
    }
}