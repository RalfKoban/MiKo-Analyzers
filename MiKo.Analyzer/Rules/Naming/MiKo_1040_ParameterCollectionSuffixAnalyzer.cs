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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Parameters.Length > 0 && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var parameter in symbol.Parameters)
            {
                var parameterType = parameter.Type;

                if (parameterType.IsString() || parameterType.IsEnumerable())
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
}