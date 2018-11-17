using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3022_CancellationTokenSourceParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3022";

        private static readonly string CancellationTokenSource = typeof(System.Threading.CancellationTokenSource).FullName;

        public MiKo_3022_CancellationTokenSourceParameterAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.IsOverride) return Enumerable.Empty<Diagnostic>();

            foreach (var parameter in method.Parameters)
            {
                var parameterType = parameter.Type;
                if (parameterType.TypeKind == TypeKind.Class && parameterType.ToString() == CancellationTokenSource)
                {
                    return new[] { ReportIssue(parameter, nameof(System.Threading.CancellationToken)) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}