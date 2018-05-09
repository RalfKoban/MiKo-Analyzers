using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1014_InitMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1014";

        public MiKo_1014_InitMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var methodName = method.Name;
            if (!methodName.StartsWith("Init", StringComparison.Ordinal)) return Enumerable.Empty<Diagnostic>();
            if (methodName.StartsWith("Initialize", StringComparison.Ordinal)) return Enumerable.Empty<Diagnostic>();

            var expectedName = GetExpectedName(methodName, "Initialize");

            return new[] { ReportIssue(method, expectedName) };
        }

        private static string GetExpectedName(string methodName, string expectedName)
        {
            var i = 1;
            for (; i < methodName.Length; i++)
            {
                var character = methodName[i];
                if (character.IsUpperCase()) break;
                if (character.IsNumber()) break;
            }

            return i >= methodName.Length ? expectedName : expectedName + methodName.Substring(i);
        }
    }
}