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

        private const string Returned = "Returned";

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
                foreach (ReadOnlySpan<char> part in parts)
                {
                    if (part[0].IsUpperCase() is false)
                    {
                        return false;
                    }

                    if (part.ContainsAny(ExpectedOutcomeMarkers))
                    {
                        return true;
                    }
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
                    if (parts[0].StartsWith(If, StringComparison.Ordinal))
                    {
                        result = string.Concat(FixReturn(parts[1]), FixReturn(parts[0]));
                    }
                    else if (parts[1].Equals(Returned, StringComparison.OrdinalIgnoreCase))
                    {
                        result = string.Concat(FixReturn(parts[1]), FixReturn(parts[0]));
                    }
                    else
                    {
                        result = string.Concat(FixReturn(parts[1]), If, FixReturn(parts[0]));
                    }

                    return true;
                }

                case 3:
                {
                    if (parts[1].StartsWith(If, StringComparison.Ordinal))
                    {
                        result = string.Concat(FixReturn(parts[0]), FixReturn(parts[2]), FixReturn(parts[1]));
                    }
                    else if (parts[2].Equals(Returned, StringComparison.OrdinalIgnoreCase))
                    {
                        result = string.Concat(FixReturn(parts[0]), FixReturn(parts[2]), FixReturn(parts[1]));
                    }
                    else
                    {
                        result = string.Concat(FixReturn(parts[0]), FixReturn(parts[2]), If, FixReturn(parts[1]));
                    }

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
                    var isReturned = parts[3].Equals(Returned, StringComparison.OrdinalIgnoreCase);
                    var addIf = useWhen is false && isReturned is false && parts[2].StartsWith(If, StringComparison.Ordinal) is false;

                    var capacity = parts[0].Length + parts[1].Length + parts[2].Length + parts[3].Length + And.Length;

                    if (addIf)
                    {
                        capacity += If.Length;
                    }

                    var builder = new StringBuilder(capacity).Append(FixReturn(parts[0])).Append(FixReturn(parts[3]));

                    if (addIf)
                    {
                        builder.Append(If);
                    }

                    if (useWhen)
                    {
                        builder.Append(FixReturn(parts[1])).Append(FixReturn(parts[2]));
                    }
                    else
                    {
                        builder.Append(FixReturn(parts[2])).Append(And).Append(FixReturn(parts[1]));
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

        private static string FixReturn(string original)
        {
            if (original.EndsWith(Returned, StringComparison.OrdinalIgnoreCase))
            {
                return "Returns" + original.Substring(0, original.Length - Returned.Length);
            }

            return original;
        }
    }
}