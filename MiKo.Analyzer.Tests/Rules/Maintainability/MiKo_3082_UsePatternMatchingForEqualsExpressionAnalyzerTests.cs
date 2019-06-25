using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3082_UsePatternMatchingForEqualsExpressionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Operators =
            {
                "==",
                "!=",
            };

        private static readonly string[] BooleanValues =
            {
                "true",
                "false",
            };

        [Test, Combinatorial]
        public void No_issue_is_reported_for_comparisons_of_an_(
                                                            [Values("int", "string", "object")] string type,
                                                            [ValueSource(nameof(Operators))] string @operator)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + @" a, " + type + @" b)
    {
        if (a " + @operator + @" b)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_comparisons_of_2_boolean_values_([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a, bool b)
    {
        if (a " + @operator + @" b)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_of_a_boolean_to_([ValueSource(nameof(BooleanValues))] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
        if (" + value + @" == a)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_right_sided_comparison_of_a_boolean_to_([ValueSource(nameof(BooleanValues))] string value) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
        if (a == " + value + @")
            return true;
        else
            return false;
    }
}");

        protected override string GetDiagnosticId() => MiKo_3082_UsePatternMatchingForEqualsExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3082_UsePatternMatchingForEqualsExpressionAnalyzer();
    }
}