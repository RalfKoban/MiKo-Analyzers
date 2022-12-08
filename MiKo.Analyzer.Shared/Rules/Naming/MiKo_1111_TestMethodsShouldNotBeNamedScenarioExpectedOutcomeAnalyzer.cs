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
            "Actual",
            "Expect",
            "IsEmpty",
            "IsExceptional",
            "IsNot",
            "IsNull",
            "Return",
            "Shall",
            "Should",
            "Throw",
            "Will",
        };

        public MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        internal static string FindBetterName(ISymbol symbol) => FindBetterName(symbol.Name);

        internal static string FindBetterName(string symbolName)
        {
            var parts = symbolName.Split(Underscores, StringSplitOptions.RemoveEmptyEntries);

            if (TryGetReversed(parts, out var reversed))
            {
                return NamesFinder.FindBetterTestName(reversed);
            }

            return NamesFinder.FindBetterTestName(symbolName);
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.Length > 10)
            {
                var parts = methodName.Split(Underscores, StringSplitOptions.RemoveEmptyEntries);

                var hasIssue = parts.Length >= 1 && parts.All(_ => _[0].IsUpperCase()) && parts.Last().ContainsAny(ExpectedOutcomeMarkers);

                if (hasIssue)
                {
                    yield return Issue(symbol);
                }
            }
        }

        private static bool TryGetReversed(string[] parts, out string reversed)
        {
            switch (parts.Length)
            {
                case 2:
                {
                    var addIf = parts[0].StartsWith("If", StringComparison.Ordinal) is false;

                    reversed = addIf
                                   ? parts[1] + "If" + parts[0]
                                   : parts[1] + parts[0];

                    return true;
                }

                case 3:
                {
                    var addIf = parts[1].StartsWith("If", StringComparison.Ordinal) is false;

                    reversed = addIf
                                   ? parts[0] + parts[2] + "If" + parts[1]
                                   : parts[0] + parts[2] + parts[1];

                    return true;
                }

                case 4:
                {
                    var addIf = parts[2].StartsWith("If", StringComparison.Ordinal) is false;

                    reversed = addIf
                                   ? parts[0] + parts[1] + parts[3] + "If" + parts[2]
                                   : parts[0] + parts[1] + parts[3] + parts[2];

                    return true;
                }

                default:
                {
                    reversed = null;

                    return false;
                }
            }
        }
    }
}