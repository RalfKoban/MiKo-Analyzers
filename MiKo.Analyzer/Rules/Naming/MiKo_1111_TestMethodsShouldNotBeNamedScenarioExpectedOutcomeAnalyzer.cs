using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1111";

        private static readonly char[] Underscores = { '_' };

        public MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            var methodName = symbol.Name;

            if (methodName.Length > 10)
            {
                var parts = methodName.Split(Underscores, StringSplitOptions.RemoveEmptyEntries);

                var partsStartUpperCase = parts.Length >= 1 && parts.All(_ => _[0].IsUpperCase());
                if (partsStartUpperCase)
                {
                    return new[] { Issue(symbol) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}