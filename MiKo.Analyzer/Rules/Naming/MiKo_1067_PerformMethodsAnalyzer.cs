using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1067_PerformMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1067";

        public MiKo_1067_PerformMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var methodName = method.Name;

            var found = ContainsPhrase(methodName) && ContainsPhrase(methodName.Remove("Performance"));

            return found
                       ? new[] { Issue(method) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static bool ContainsPhrase(string methodName, string phrase = "Perform") => methodName.Contains(phrase, StringComparison.Ordinal);
    }
}