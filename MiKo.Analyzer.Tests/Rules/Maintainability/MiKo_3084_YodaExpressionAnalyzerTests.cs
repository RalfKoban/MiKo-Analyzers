using System.Linq;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3084_YodaExpressionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] EqualityOperators =
                                                             {
                                                                 "==",
                                                                 "!=",
                                                             };

        private static readonly string[] ComparingOperators =
                                                              {
                                                                  "<=",
                                                                  ">=",
                                                                  "<",
                                                                  ">",
                                                              };

        private static readonly string[] Operators = EqualityOperators.Concat(ComparingOperators).ToArray();

        [Test, Combinatorial]
        public void No_issue_is_reported_for_comparisons_of_2_variables_of_type_(
                                                                             [Values("int", "string", "object")] string type,
                                                                             [ValueSource(nameof(EqualityOperators))] string @operator)
            => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(" + type + " a, " + type + @" b)
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

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_of_a_hardcoded_string_([ValueSource(nameof(Operators))] string @operator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
        if (""something""" + @operator + @" a)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_right_sided_comparison_of_a_hardcoded_string_([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
        if (a " + @operator + @"""something"")
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_of_a_number_([ValueSource(nameof(Operators))] string @operator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int a)
    {
        if (42 " + @operator + @" a)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_of_a_floating_number_([ValueSource(nameof(Operators))] string @operator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(float a)
    {
        if (08.15 " + @operator + @" a)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_right_sided_comparison_of_a_number_([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int a)
    {
        if (a " + @operator + @" 42)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_right_sided_comparison_of_a_floating_number_([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(float a)
    {
        if (a " + @operator + @" 08.15)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_of_a_const_([ValueSource(nameof(Operators))] string @operator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private const int VALUE = 42;

    public bool DoSomething(int a)
    {
        if (VALUE " + @operator + @" a)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_right_sided_comparison_of_a_const_([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private const int VALUE = 42;

    public bool DoSomething(int a)
    {
        if (a " + @operator + @" VALUE)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_of_an_enum_member_([ValueSource(nameof(Operators))] string @operator) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(GCCollectionMode a)
    {
        if (GCCollectionMode.Default " + @operator + @" a)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_right_sided_comparison_of_an_enum_member_([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(GCCollectionMode a)
    {
        if (a " + @operator + @" GCCollectionMode.Default)
            return true;
        else
            return false;
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_left_sided_comparison_of_an_property_named_after_an_enum_([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using System;

public class Helper
{
    public GCCollectionMode GCCollectionMode { get; set; }
}

public class TestMe
{
    public bool DoSomething(GCCollectionMode a)
    {
        var helper = new Helper();
        if (helper.GCCollectionMode " + @operator + @" a)
            return true;
        else
            return false;
    }
}");

        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A == a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a == A; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A != a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a != A; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A <= a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a >= A; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A < a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a > A; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A >= a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a <= A; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A > a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a < A; } }")]
        [TestCase("class TestMe { const int A = -42; bool Do(object a) { return A > a; } }", "class TestMe { const int A = -42; bool Do(object a) { return a < A; } }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_3084_YodaExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3084_YodaExpressionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3084_CodeFixProvider();
    }
}