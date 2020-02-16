using System.Collections.Generic;

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

            if (parameters[0].Name != "sender")
            {
                yield return Issue(parameters[0], "sender");
            }

            if (parameters[1].Name != "e")
            {
                yield return Issue(parameters[1], "e");
            }
        }
    }
}