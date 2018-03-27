using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1003_EventHandlingMethodNamePrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1003";

        public MiKo_1003_EventHandlingMethodNamePrefixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeOrdinaryMethod(IMethodSymbol method)
        {
            if (!method.IsEventHandler()) return Enumerable.Empty<Diagnostic>();

            return method.Name.StartsWith("On", StringComparison.Ordinal)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(method, FindProperName(method)) };
        }

        private static string FindProperName(IMethodSymbol method) => "On" + method.Name.Replace("_", string.Empty);
    }
}