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

        private static readonly string[] ExpectedOutcomeMarkers =
        {
            "IsExceptional",
            "Return",
            "Throw",
        };

        public MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        internal static string FindBetterName(ISymbol symbol)
        {
            var parts = symbol.Name.Split(Underscores, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var addIf = parts[0].StartsWith("If") is false;

                var reversed = addIf
                                ? parts[1] + "If" + parts[0]
                                : parts[1] + parts[0];

                return NamesFinder.FindBetterTestName(reversed);
            }

            return symbol.Name;
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.Length > 10)
            {
                var parts = methodName.Split(Underscores, StringSplitOptions.RemoveEmptyEntries);

                var partsStartUpperCase = parts.Length >= 1 && parts.All(_ => _[0].IsUpperCase());
                if (partsStartUpperCase)
                {
                    yield return Issue(symbol);
                }
            }
        }
    }
}