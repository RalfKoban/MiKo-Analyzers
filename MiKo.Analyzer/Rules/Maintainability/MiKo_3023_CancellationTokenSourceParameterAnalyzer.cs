using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3023_CancellationTokenSourceParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3023";

        public MiKo_3023_CancellationTokenSourceParameterAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method)
        {
            foreach (var parameter in method.Parameters)
            {
                var parameterType = parameter.Type;
                if (parameterType.TypeKind == TypeKind.Class && parameterType.ToString() == TypeNames.CancellationTokenSource)
                {
                    return new[] { Issue(parameter, nameof(CancellationToken)) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}