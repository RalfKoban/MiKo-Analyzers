using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1115";

        private const string If = "If";
        private const string IfIt = "If_It";
        private const string Is = "Is";
        private const string When = "When";
        private const string And = "And";
        private const string Returned = "Returned";
        private const string Returns = "Returns";
        private const string Throws = "Throws";
        private const string Throw = "Throw";
        private const string NoLongerThrows = "NoLongerThrows";
        private const string NoLongerThrow = "NoLongerThrow";
        private const string NotThrows = "NotThrows";
        private const string NotThrow = "NotThrow";
        private const string Given = "Given";
        private const string IsGiven = Is + Given;
        private const string Consumed = "Consumed";
        private const string Rejected = "Rejected";

        private static readonly string[] ExpectedOutcomeMarkers =
                                                                  {
                                                                      "Actual",
                                                                      "Expect",
                                                                      Is + "Empty",
                                                                      Is + "Exceptional",
                                                                      Is + "Not",
                                                                      Is + "Null",
                                                                      "Return",
                                                                      "Shall",
                                                                      "Should",
                                                                      "Throw",
                                                                      "Will",
                                                                      "Try",
                                                                      "Tries",
                                                                      "Call",
                                                                      "Invoke",
                                                                      Given,
                                                                      "Everything",
                                                                      Rejected,
                                                                      Consumed,
                                                                  };

        private static readonly string[] SpecialFirstPhrases =
                                                               {
                                                                   Returns,
                                                                   Throws,
                                                                   Throw,
                                                                   NotThrows,
                                                                   NotThrow,
                                                                   NoLongerThrows,
                                                                   NoLongerThrow,
                                                               };

        private static readonly string[] SpecialThrowPhrases =
                                                               {
                                                                   Throws,
                                                                   Throw,
                                                                   NotThrows,
                                                                   NotThrow,
                                                                   NoLongerThrows,
                                                                   NoLongerThrow,
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
                var betterName = FindBetterName(methodName);

                return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private static bool HasIssue(string methodName)
        {
            var parts = methodName.Split(Constants.Underscores, StringSplitOptions.RemoveEmptyEntries);
            var first = true;

            foreach (var part in parts)
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

        private static string FindBetterName(string symbolName)
        {
            var name = symbolName.Replace("_Expect_", "_");

            if (TryGetInOrder(name, out var nameInOrder))
            {
                return NamesFinder.FindBetterTestName(nameInOrder);
            }

            return NamesFinder.FindBetterTestName(name);
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
            if (part1.StartsWithAny(SpecialThrowPhrases, StringComparison.OrdinalIgnoreCase) && part1.Contains(If, StringComparison.Ordinal))
            {
                // it seems like this is in normal order, so do not change the order
                result = null;

                return false;
            }

            var addIf = IsIfRequired(part0, part1);
            var ifToAdd = Verbalizer.IsThirdPersonSingularVerb(part0.AsSpan().FirstWord()) ? IfIt : If;

            var capacity = part0.Length + part1.Length;

            if (addIf)
            {
                capacity += ifToAdd.Length;
            }

            var builder = new StringBuilder(capacity);
            FixPart(builder, part1);

            if (addIf)
            {
                builder.Append(ifToAdd);
            }

            FixPart(builder, part0);

            builder.ReplaceWithCheck(When, If).ReplaceWithCheck(If + If, If);

            result = builder.ToString();

            return true;
        }

        private static bool TryGetInOrder(string part0, string part1, string part2, out string result)
        {
            if (part1.StartsWithAny(SpecialThrowPhrases, StringComparison.OrdinalIgnoreCase))
            {
                // it seems like this is in normal order, so do not change the order
                result = null;

                return false;
            }

            var addIf = IsIfRequired(part1, part2);
            var ifToAdd = Verbalizer.IsThirdPersonSingularVerb(part1.AsSpan().FirstWord()) ? IfIt : If;

            var capacity = part0.Length + part1.Length + part2.Length;

            if (addIf)
            {
                capacity += ifToAdd.Length;
            }

            var builder = new StringBuilder(capacity);
            FixPart(builder, part0);
            FixPart(builder, part2);

            if (addIf)
            {
                builder.Append(ifToAdd);
            }

            if (part1.StartsWith(Given, StringComparison.OrdinalIgnoreCase))
            {
                part1 = part1.AsSpan(Given.Length).ConcatenatedWith(IsGiven);
            }

            FixPart(builder, part1);

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

            if (part1.StartsWithAny(SpecialThrowPhrases, StringComparison.OrdinalIgnoreCase))
            {
                // it seems like this is in normal order, so do not change the order
                result = null;

                return false;
            }

            var useWhen = part1.StartsWith(When, StringComparison.Ordinal);

            var addIf = useWhen is false && IsIfRequired(part2, part3);
            var ifToAdd = Verbalizer.IsThirdPersonSingularVerb((useWhen ? part1 : part2).AsSpan().FirstWord()) ? IfIt : If;

            var capacity = part0.Length + part1.Length + part2.Length + part3.Length + And.Length;

            if (addIf)
            {
                capacity += ifToAdd.Length;
            }

            var builder = new StringBuilder(capacity);
            FixPart(builder, part0);
            FixPart(builder, part3);

            if (addIf)
            {
                builder.Append(ifToAdd);
            }

            if (useWhen)
            {
                FixPart(builder, part1);
                FixPart(builder, part2);
            }
            else
            {
                FixPart(builder, part2);
                builder.Append(And);
                FixPart(builder, part1);
            }

            builder.ReplaceWithCheck(When, If).ReplaceWithCheck(If + If, If);

            result = builder.ToString();

            return true;
        }

        private static bool IsIfRequired(string part1, string part2)
        {
            if (part1.StartsWith(If, StringComparison.OrdinalIgnoreCase)
             || part1.StartsWith(Consumed, StringComparison.OrdinalIgnoreCase)
             || part2.Equals(Returned, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private static void FixPart(StringBuilder builder, string original)
        {
            if (original.EndsWith(Returned, StringComparison.OrdinalIgnoreCase))
            {
                builder.Append(Returns).Append(original, 0, original.Length - Returned.Length);
            }
            else if (original.StartsWith(Rejected, StringComparison.OrdinalIgnoreCase))
            {
                builder.Append(original, Rejected.Length, original.Length - Rejected.Length).Append(Is).Append(Rejected);
            }
            else
            {
                builder.Append(original);
            }
        }
    }
}