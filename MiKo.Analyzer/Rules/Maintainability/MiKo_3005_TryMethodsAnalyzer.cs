using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3005_TryMethodsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3005";

        public MiKo_3005_TryMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.IsOverride) return Enumerable.Empty<Diagnostic>();

            if (!method.Name.StartsWith("Try", StringComparison.Ordinal)) return Enumerable.Empty<Diagnostic>();

            if (method.ReturnType.SpecialType == SpecialType.System_Boolean && (method.Parameters.Any() && method.Parameters.Last().RefKind == RefKind.Out))
            {
                // correct
                return Enumerable.Empty<Diagnostic>();
            }

            return new[] { ReportIssue(method) };
        }
    }
}