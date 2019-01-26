using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1014_CheckMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1014";

        private static readonly string[] StartingPhrases = { "CheckIn", "CheckOut" };

        public MiKo_1014_CheckMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var methodName = method.Name;
            var forbidden = methodName.StartsWith("Check", StringComparison.Ordinal) && !methodName.StartsWithAny(StartingPhrases, StringComparison.Ordinal);
            return forbidden
                       ? new[] { ReportIssue(method) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}