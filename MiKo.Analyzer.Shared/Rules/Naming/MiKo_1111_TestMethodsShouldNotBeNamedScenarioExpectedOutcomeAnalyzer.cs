using System;
using System.Collections.Generic;
using System.Text;

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

            if (TryGetInOrder(parts, out var nameInOrder))
            {
                return NamesFinder.FindBetterTestName(nameInOrder);
            }

            return NamesFinder.FindBetterTestName(symbolName);
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.Length > 10)
            {
                if (HasIssue(methodName))
                {
                    yield return Issue(symbol);
                }
            }
        }

        private static bool HasIssue(string methodName)
        {
            var parts = methodName.SplitBy(Underscores, StringSplitOptions.RemoveEmptyEntries);
            var count = parts.Count();

            if (count >= 1)
            {
                var last = count - 1;
                var index = 0;

                foreach (ReadOnlySpan<char> part in parts)
                {
                    if (part[0].IsUpperCase() is false)
                    {
                        return false;
                    }

                    // if (index == last)
                    {
                        if (part.ContainsAny(ExpectedOutcomeMarkers))
                        {
                            return true;
                        }
                    }

                    index++;
                }
            }

            return false;
        }

        private static bool TryGetInOrder(string[] parts, out string result)
        {
            const string If = "If";

            switch (parts.Length)
            {
                case 2:
                {
                    result = parts[0].StartsWith(If, StringComparison.Ordinal)
                               ? string.Concat(parts[1], parts[0])
                               : string.Concat(parts[1], If, parts[0]); // add if

                    return true;
                }

                case 3:
                {
                    result = parts[1].StartsWith(If, StringComparison.Ordinal)
                               ? string.Concat(parts[0], parts[2], parts[1])
                               : string.Concat(parts[0], parts[2], If, parts[1]); // add if

                    return true;
                }

                case 4:
                {
                    const string And = "And";

                    if (parts[2].StartsWith(And, StringComparison.Ordinal))
                    {
                        // it seems like this is in normal order and a combination of 2 scenarios, so do not change the order
                        result = null;

                        return false;
                    }

                    var useWhen = parts[1].StartsWith("When", StringComparison.Ordinal);
                    var addIf = useWhen is false && parts[2].StartsWith(If, StringComparison.Ordinal) is false;

                    var capacity = parts[0].Length + parts[1].Length + parts[2].Length + parts[3].Length + And.Length;

                    if (addIf)
                    {
                        capacity += If.Length;
                    }

                    var builder = new StringBuilder(capacity).Append(parts[0]).Append(parts[3]);

                    if (addIf)
                    {
                        builder.Append(If);
                    }

                    if (useWhen)
                    {
                        builder.Append(parts[1]).Append(parts[2]);
                    }
                    else
                    {
                        builder.Append(parts[2]).Append(And).Append(parts[1]);
                    }

                    result = builder.ToString();

                    return true;
                }

                default:
                {
                    result = null;

                    return false;
                }
            }
        }
    }
}