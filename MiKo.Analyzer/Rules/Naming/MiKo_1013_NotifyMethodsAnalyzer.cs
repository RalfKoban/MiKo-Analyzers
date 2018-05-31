using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1013_NotifyMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1013";

        public MiKo_1013_NotifyMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method) => method.Name.StartsWithAny(StringComparison.Ordinal, "Notify", "OnNotify")
                                                                                            ? new[] { ReportIssue(method) }
                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}