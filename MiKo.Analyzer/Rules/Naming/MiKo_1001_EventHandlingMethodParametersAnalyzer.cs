using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Extensions;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1001_EventHandlingMethodParametersAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1001";

        public MiKo_1001_EventHandlingMethodParametersAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (!method.IsEventHandler()) return Enumerable.Empty<Diagnostic>();

            var parameters = method.Parameters;

            var diagnostics = new List<Diagnostic>();
            VerifyParameterName("sender", parameters[0].Name, method, diagnostics);
            VerifyParameterName("e", parameters[1].Name, method, diagnostics);
            return diagnostics;
        }

        private void VerifyParameterName(string expected, string actual, IMethodSymbol method, ICollection<Diagnostic> diagnostics)
        {
            if (expected != actual)
            {
                diagnostics.Add(ReportIssue(method, actual, expected));
            }
        }
    }
}