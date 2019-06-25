using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1002_EventHandlingMethodParametersAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1002";

        public MiKo_1002_EventHandlingMethodParametersAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => method.IsEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var parameters = method.Parameters;

            List<Diagnostic> diagnostics = null;
            VerifyParameterName("sender", parameters[0], ref diagnostics);
            VerifyParameterName("e", parameters[1], ref diagnostics);
            return diagnostics ?? Enumerable.Empty<Diagnostic>();
        }

        private void VerifyParameterName(string expected, IParameterSymbol parameter, ref List<Diagnostic> diagnostics)
        {
            if (expected == parameter.Name) return;

            if (diagnostics is null) diagnostics = new List<Diagnostic>();

            diagnostics.Add(Issue(parameter, expected));
        }
    }
}