using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1115";

        private const string If = "If";
        private const string When = "When";
        private const string And = "And";
        private const string Returned = "Returned";
        private const string Returns = "Returns";
        private const string Throws = "Throws";

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

        private static readonly string[] SpecialFirstPhrases =
                                                               {
                                                                   Returns,
                                                                   Throws,
                                                               };

        public MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer() : base(Id)
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
            var parts = methodName.SplitBy(Constants.Underscores, StringSplitOptions.RemoveEmptyEntries);
            var first = true;

            foreach (ReadOnlySpan<char> part in parts)
            {
                if (part[0].IsUpperCase() is false)
                {
                    return false;
                }

                // jump over first part
                if (first && part.StartsWithAny(SpecialFirstPhrases, StringComparison.Ordinal))
                {
                    first = false;

                    continue;
                }

                first = false;

                if (part.ContainsAny(ExpectedOutcomeMarkers, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetInOrder(string name, out string result)
        {
            var parts = name.Split(Constants.Underscores, StringSplitOptions.RemoveEmptyEntries);

            switch (parts.Length)
            {
                case 2: return TryGetInOrder(parts[0], parts[1], out result);
                case 3: return TryGetInOrder(parts[0], parts[1], parts[2], out result);
                case 4: return TryGetInOrder(parts[0], parts[1], parts[2], parts[3], out result);

                default:
                {
                    result = null;

                    return false;
                }
            }
        }

        private static bool TryGetInOrder(string part0, string part1, out string result)
        {
            if (part1.StartsWith(Throws, StringComparison.OrdinalIgnoreCase) && part1.Contains(If, StringComparison.Ordinal))
            {
                // it seems like this is in normal order, so do not change the order
                result = null;

                return false;
            }

            var addIf = IsIfRequired(part0, part1);

            var builder = new StringBuilder(part0.Length + part1.Length);
            FixReturn(builder, part1);

            if (addIf)
            {
                builder.Append(If);
            }

            FixReturn(builder, part0);

            builder.ReplaceWithCheck(When, If).ReplaceWithCheck(If + If, If);

            result = builder.ToString();

            return true;
        }

        private static bool TryGetInOrder(string part0, string part1, string part2, out string result)
        {
            if (part1.StartsWith(Throws, StringComparison.OrdinalIgnoreCase))
            {
                // it seems like this is in normal order, so do not change the order
                result = null;

                return false;
            }

            var addIf = IsIfRequired(part1, part2);

            var builder = new StringBuilder(part0.Length + part1.Length + part2.Length);
            FixReturn(builder, part0);
            FixReturn(builder, part2);

            if (addIf)
            {
                builder.Append(If);
            }

            FixReturn(builder, part1);

            builder.ReplaceWithCheck(When, If).ReplaceWithCheck(If + If, If);

            result = builder.ToString();

            return true;
        }

        private static bool TryGetInOrder(string part0, string part1, string part2, string part3, out string result)
        {
            if (part2.StartsWith(And, StringComparison.Ordinal))
            {
                // it seems like this is in normal order and a combination of 2 scenarios, so do not change the order
                result = null;

                return false;
            }

            if (part1.StartsWith(Throws, StringComparison.OrdinalIgnoreCase))
            {
                // it seems like this is in normal order, so do not change the order
                result = null;

                return false;
            }

            var useWhen = part1.StartsWith(When, StringComparison.Ordinal);
            var addIf = useWhen is false && IsIfRequired(part2, part3);

            var capacity = part0.Length + part1.Length + part2.Length + part3.Length + And.Length;

            if (addIf)
            {
                capacity += If.Length;
            }

            var builder = new StringBuilder(capacity);
            FixReturn(builder, part0);
            FixReturn(builder, part3);

            if (addIf)
            {
                builder.Append(If);
            }

            if (useWhen)
            {
                FixReturn(builder, part1);
                FixReturn(builder, part2);
            }
            else
            {
                FixReturn(builder, part2);
                builder.Append(And);
                FixReturn(builder, part1);
            }

            builder.ReplaceWithCheck(When, If).ReplaceWithCheck(If + If, If);

            result = builder.ToString();

            return true;
        }

        private static bool IsIfRequired(string part1, string part2)
        {
            if (part1.StartsWith(If, StringComparison.OrdinalIgnoreCase) || part2.Equals(Returned, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private static void FixReturn(StringBuilder builder, string original)
        {
            if (original.EndsWith(Returned, StringComparison.OrdinalIgnoreCase))
            {
                builder.Append(Returns).Append(original, 0, original.Length - Returned.Length);
            }
            else
            {
                builder.Append(original);
            }
        }
    }
}