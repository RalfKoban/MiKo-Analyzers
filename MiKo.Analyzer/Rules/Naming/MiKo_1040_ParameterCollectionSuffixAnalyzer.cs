using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1040_ParameterCollectionSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1040";

        public MiKo_1040_ParameterCollectionSuffixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => method.Parameters.Length > 0;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            foreach (var parameter in method.Parameters)
            {
                var diagnostic = AnalyzeCollectionSuffix(parameter);
                if (diagnostic != null)
                {
                    yield return diagnostic;
                }
            }
        }
    }
}