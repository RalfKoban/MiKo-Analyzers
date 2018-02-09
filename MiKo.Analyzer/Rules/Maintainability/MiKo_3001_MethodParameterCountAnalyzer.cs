using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3001_MethodParameterCountAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3001";

        private const int MaxParametersCount = 5;

        public MiKo_3001_MethodParameterCountAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            var parameterCount = method.Parameters.Count();
            if (parameterCount <= MaxParametersCount) return Enumerable.Empty<Diagnostic>();

            return new[] { ReportIssue(method, parameterCount, MaxParametersCount) };
        }
    }
}