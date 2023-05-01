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
        private const string If = "If";
        private const string When = "When";
        private const string And = "And";

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
            var name = symbolName.Replace("_Expect_", "_");

            if (TryGetInOrder(name, out var nameInOrder))
            {
                return NamesFinder.FindBetterTestName(nameInOrder);
            }

            return NamesFinder.FindBetterTestName(name);
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

                    if (part.ContainsAny(ExpectedOutcomeMarkers, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryGetInOrder(string name, out string result)
        {
            var parts = name.Split(Underscores, StringSplitOptions.RemoveEmptyEntries);

            switch (parts.Length)
            {
                case 2:
                {
                    if (parts[1].Contains("ThrowsExceptionIf", StringComparison.OrdinalIgnoreCase))
                    {
                        // it seems like this is in normal order, so do not change the order
                        result = null;

                        return false;
                    }

                    var addIf = IsIfRequired(parts[0], parts[1]);

                    var builder = new StringBuilder(FixReturn(parts[1]));

                    if (addIf)
                    {
                        builder.Append(If);
                    }

                    builder.Append(FixReturn(parts[0]));

                    builder.Replace(When, If).Replace(If + If, If);

                    result = builder.ToString();

                    return true;
                }

                case 3:
                {
                    var addIf = IsIfRequired(parts[1], parts[2]);

                    var builder = new StringBuilder(FixReturn(parts[0])).Append(FixReturn(parts[2]));

                    if (addIf)
                    {
                        builder.Append(If);
                    }

                    builder.Append(FixReturn(parts[1]));

                    builder.Replace(When, If).Replace(If + If, If);

                    result = builder.ToString();

                    return true;
                }

                case 4:
                {
                    if (parts[2].StartsWith(And, StringComparison.Ordinal))
                    {
                        // it seems like this is in normal order and a combination of 2 scenarios, so do not change the order
                        result = null;

                        return false;
                    }

                    var useWhen = parts[1].StartsWith(When, StringComparison.Ordinal);
                    var addIf = useWhen is false && IsIfRequired(parts[2], parts[3]);

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

                    builder.Replace(When, If).Replace(If + If, If);

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

        private static bool IsIfRequired(string part1, string part2) => part1.StartsWith(If, StringComparison.Ordinal) is false
                                                                     && part2.Equals(Returned, StringComparison.OrdinalIgnoreCase) is false;

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