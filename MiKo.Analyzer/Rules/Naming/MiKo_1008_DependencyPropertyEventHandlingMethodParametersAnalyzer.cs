using System.Collections.Generic;

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

            if (parameters[0].Name != "d")
            {
                yield return Issue(parameters[0], "d");
            }

            if (parameters[1].Name != "e")
            {
                yield return Issue(parameters[1], "e");
            }
        }
    }
}