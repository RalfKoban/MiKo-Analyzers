using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1013_CheckMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1013";

        public MiKo_1013_CheckMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.MethodKind != MethodKind.Ordinary || method.IsOverride) return Enumerable.Empty<Diagnostic>();

            var methodName = method.Name;
            var forbidden = methodName.StartsWith("Check", StringComparison.Ordinal) && !methodName.StartsWithAny(StringComparison.Ordinal, "CheckIn", "CheckOut");
            return forbidden
                       ? new[] { ReportIssue(method) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}