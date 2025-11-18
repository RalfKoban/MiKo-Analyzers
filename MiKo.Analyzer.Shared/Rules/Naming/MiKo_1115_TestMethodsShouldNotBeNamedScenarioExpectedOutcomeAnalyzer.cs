using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1115";

        private const string SpecialMarkerHandling = "Create";

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
                                                                      "Try",
                                                                      "Tries",
                                                                      "Call",
                                                                      "Invoke",
                                                                      "Given",
                                                                      "Everything",
                                                                      "Rejected",
                                                                      "Consumed",
                                                                      "Once",
                                                                      "Does", // incl. 'DoesNot'
                                                                      SpecialMarkerHandling,
                                                                      "Creates",
                                                                      "Append",
                                                                      "Keep",
                                                                      "Accepted",
                                                                  };

        private static readonly string[] SpecialFirstPhrases =
                                                               {
                                                                   "Returns",
                                                                   "Throws",
                                                                   "Throw",
                                                                   "NotThrows",
                                                                   "NotThrow",
                                                                   "NoLongerThrows",
                                                                   "NoLongerThrow",
                                                               };

        public MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.Length > 10 && HasIssue(methodName))
            {
                var betterName = NamesFinder.FindBetterTestNameWithReorder(methodName, symbol);

                return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static bool HasIssue(string methodName)
        {
            var parts = methodName.Split(Constants.Underscores, StringSplitOptions.RemoveEmptyEntries);
            var first = true;

            foreach (var part in parts)
            {
                if (part[0].IsUpperCase() is false && part[0].IsNumber() is false)
                {
                    return false;
                }

                // jump over first part
                if (first && part.StartsWithAny(SpecialFirstPhrases))
                {
                    first = false;

                    continue;
                }

                for (int index = 0, length = ExpectedOutcomeMarkers.Length; index < length; index++)
                {
                    var marker = ExpectedOutcomeMarkers[index];

                    if (part.Contains(marker, StringComparison.OrdinalIgnoreCase))
                    {
                        if (first && SpecialMarkerHandling.Equals(marker, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        return true;
                    }
                }

                first = false;
            }

            return false;
        }
    }
}