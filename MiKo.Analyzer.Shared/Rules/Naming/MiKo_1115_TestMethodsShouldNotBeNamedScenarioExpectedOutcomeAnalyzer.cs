using System;
using System.Collections.Concurrent;
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
                                                                      "Create",
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

        private static readonly string[] SpecialConditionPhrases =
                                                                   {
                                                                       "If",
                                                                       "When",
                                                                   };

        private static readonly ConcurrentDictionary<string, (bool HasIssue, string BetterName)> BetterNamesCache = new ConcurrentDictionary<string, (bool, string)>();

        public MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.Length > 10)
            {
                var hasIssue = false;
                var betterName = string.Empty;

                if (BetterNamesCache.TryGetValue(methodName, out var cachedValue))
                {
                    if (cachedValue.HasIssue)
                    {
                        hasIssue = true;
                        betterName = cachedValue.BetterName;
                    }
                }
                else
                {
                    if (HasIssue(methodName))
                    {
                        hasIssue = true;
                        betterName = NamesFinder.FindBetterTestNameWithReorder(methodName, symbol);
                    }

                    BetterNamesCache.TryAdd(methodName, (hasIssue, betterName));
                }

                if (hasIssue)
                {
                    return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static bool HasIssue(string methodName)
        {
            var first = true;
            var index = -1;

            foreach (ReadOnlySpan<char> part in methodName.AsSpan().SplitBy(Constants.Underscores, StringSplitOptions.RemoveEmptyEntries))
            {
                index++;

                if (part[0].IsUpperCaseOrNumber() is false)
                {
                    return false;
                }

                // jump over first part
                if (first)
                {
                    first = false;

                    if (part.StartsWith("Create"))
                    {
                        if (part.Length is 6 || part[6].IsUpperCase())
                        {
                            continue; // we allow 'Create' methods
                        }
                    }
                    else if (part.StartsWith("Try"))
                    {
                        if (part.Length is 3 || part[3].IsUpperCase())
                        {
                            continue; // we allow 'Try' methods
                        }
                    }
                    else if (part.StartsWithAny(SpecialFirstPhrases))
                    {
                        continue;
                    }
                }
                else
                {
                    if (index is 1 && part.StartsWithAny(SpecialConditionPhrases))
                    {
                        return true;
                    }
                }

                if (part.ContainsAny(ExpectedOutcomeMarkers, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}