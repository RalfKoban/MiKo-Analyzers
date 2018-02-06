using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Extensions;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1002_EventArgsParameterAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1002";

        public MiKo_1002_EventArgsParameterAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.IsOverride) return Enumerable.Empty<Diagnostic>();

            // ignore the method as it is handled by EventHandlingMethodParametersAnalyzer
            if (method.IsEventHandler()) return Enumerable.Empty<Diagnostic>();

            var diagnostics = method.Parameters
                                    .Where(_ => _.Type.InheritsFrom<System.EventArgs>() && _.Name != "e")
                                    .Select(_ => ReportIssue(method, _.Name))
                                    .ToList();
            return diagnostics;
        }
    }
}