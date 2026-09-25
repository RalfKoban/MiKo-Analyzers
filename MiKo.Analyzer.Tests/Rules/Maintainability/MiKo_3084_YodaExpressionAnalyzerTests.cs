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
                                                             [
                                                                 "==",
                                                                 "!=",
                                                             ];

        private static readonly string[] ComparingOperators =
                                                              [
                                                                  "<=",
                                                                  ">=",
                                                                  "<",
                                                                  ">",
                                                              ];

        private static readonly string[] Operators = [.. EqualityOperators, .. ComparingOperators];

        [Test]
        public void No_issue_is_reported_for_a_comparison_of_2_variables_via_Equals() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int a, int b)
    {
        return a.Equals(b);
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
        public void No_issue_is_reported_for_a_right_sided_comparison_via_Equals_of_a_hardcoded_string() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(string a)
    {
        return a.Equals(""something"");
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_right_sided_comparison_via_Equals_of_a_number() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int a)
    {
        return a.Equals(42);
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_static_Equals_call_with_hardcoded_values() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object a)
    {
        return Equals(42, a);
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_static_object_Equals_call_with_a_hardcoded_value() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(object a)
    {
        return object.Equals(42, a);
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_static_string_Equals_call_with_a_hardcoded_string() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(string a)
    {
        return string.Equals(""something"", a);
    }
}");

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
        public void An_issue_is_reported_for_a_left_sided_comparison_via_Equals_of_a_bool() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(bool a)
    {
        return true.Equals(a);
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_via_Equals_of_a_const() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    private const int VALUE = 42;

    public bool DoSomething(int a)
    {
        return VALUE.Equals(a);
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_via_Equals_of_a_hardcoded_string() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(string a)
    {
        return ""something"".Equals(a);
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_via_Equals_of_a_number() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(int a)
    {
        return 42.Equals(a);
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_left_sided_comparison_via_Equals_of_an_enum_member() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public bool DoSomething(GCCollectionMode a)
    {
        return GCCollectionMode.Default.Equals(a);
    }
}");

        // operators
        [TestCase("class TestMe { bool Do(object a) { return null == a; } }", "class TestMe { bool Do(object a) { return a == null; } }")]
        [TestCase("class TestMe { bool Do(object a) { return 42 == a; } }", "class TestMe { bool Do(object a) { return a == 42; } }")]
        [TestCase("class TestMe { bool Do(object a) { return -42 == a; } }", "class TestMe { bool Do(object a) { return a == -42; } }")]
        [TestCase("class TestMe { bool Do(object a) { return +42 == a; } }", "class TestMe { bool Do(object a) { return a == +42; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A == a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a == A; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A != a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a != A; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A <= a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a >= A; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A < a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a > A; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A >= a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a <= A; } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A > a; } }", "class TestMe { const int A = 42; bool Do(object a) { return a < A; } }")]
        [TestCase("class TestMe { const int A = -42; bool Do(object a) { return A > a; } }", "class TestMe { const int A = -42; bool Do(object a) { return a < A; } }")]
        [TestCase("using System; class TestMe { bool Do(GCCollectionMode a) => GCCollectionMode.Default == a; }", "using System; class TestMe { bool Do(GCCollectionMode a) => a == GCCollectionMode.Default; }")]

        // Equals
        [TestCase("class TestMe { bool Do(object a) { return 42.Equals(a); } }", "class TestMe { bool Do(object a) { return a.Equals(42); } }")]
        [TestCase("""class TestMe { bool Do(object a) { return "something".Equals(a); } }""", """class TestMe { bool Do(object a) { return a.Equals("something"); } }""")]
        [TestCase("class TestMe { bool Do(object a) { return true.Equals(a); } }", "class TestMe { bool Do(object a) { return a.Equals(true); } }")]
        [TestCase("class TestMe { const int A = 42; bool Do(object a) { return A.Equals(a); } }", "class TestMe { const int A = 42; bool Do(object a) { return a.Equals(A); } }")]
        [TestCase("using System; class TestMe { bool Do(GCCollectionMode a) => GCCollectionMode.Default.Equals(a); }", "using System; class TestMe { bool Do(GCCollectionMode a) => a.Equals(GCCollectionMode.Default); }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_3084_YodaExpressionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3084_YodaExpressionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3084_CodeFixProvider();
    }
}