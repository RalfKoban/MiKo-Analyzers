using System.Collections.Generic;
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

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var parameters = symbol.Parameters;
            var length = parameters.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var parameter = parameters[index];
                    var parameterType = parameter.Type;

                    if (parameterType.TypeKind == TypeKind.Class && parameterType.ToString() == TypeNames.CancellationTokenSource)
                    {
                        yield return Issue(parameter, nameof(CancellationToken));
                    }
                }
            }
        }
    }
}