using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3006_CancellationTokenParameterPositionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3006";

        public MiKo_3006_CancellationTokenParameterPositionAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var methodParameters = symbol.Parameters;
            var last = methodParameters.Length - 1;

            for (var i = 0; i < methodParameters.Length; i++)
            {
                var parameter = methodParameters[i];

                if (parameter.Type.IsCancellationToken() && i != last && methodParameters[last].IsParams is false)
                {
                    return new[] { Issue(parameter) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}