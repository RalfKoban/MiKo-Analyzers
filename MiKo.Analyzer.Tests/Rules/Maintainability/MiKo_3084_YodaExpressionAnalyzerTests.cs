using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3084_YodaExpressionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Operators =
            {
                "==",
                "!=",
            };

        [Test, Combinatorial]
        public void No_issue_is_reported_for_comparisons_of_2_variables_of_type_(
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
        public void An_issue_is_reported_for_a_left_sided_comparison_of_null_to_a_([Values("int", "string", "object")] string type) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + @" a)
    {
        if (null == a)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_right_sided_comparison_of_null_to_a_([Values("int", "string", "object")] string type) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + @" a)
    {
        if (a == null)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_of_a_bool_to_([Values("true", "false")] string value) => An_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_a_right_sided_comparison_of_a_bool_to_([Values("true", "false")] string value) => No_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_3084_YodaExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3084_YodaExpressionAnalyzer();
    }
}