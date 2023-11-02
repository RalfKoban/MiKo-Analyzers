using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_1_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public int A() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_2_methods() => No_issue_is_reported_for(@"
public class TestMe
{
    public int A() => 42;

    public int B() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_1_public_and_1_private_method_sharing_same_name_and_1_public_method_in_between() => No_issue_is_reported_for(@"
public class TestMe
{
    public int A() => 42;

    public int B() => 42;

    private int A() => 0815;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_2_alternating_public_and_private_methods_sharing_same_name() => No_issue_is_reported_for(@"
public class TestMe
{
    public int A() => 42;

    public int B() => 42;

    private int A() => 0815;

    private int B() => 0815;
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_private_static_and_non_static_methods_sharing_same_name_and_1_method_in_between() => No_issue_is_reported_for(@"
public class TestMe
{
    private static int A() => 0815;

    private int B() => 4711;

    private int A(object o) => 0815;
}
");

        [Test]
        public void No_issue_is_reported_for_record_with_primary_constructor_and_an_additional_constructor() => No_issue_is_reported_for(@"
public record TestMe(int a, int b)
{
    public int A = a;
    public int B = b;
    public int C;

    public TestME(int a, int b, int c)
    : this(a, b)
    {
        C = c;
    }

    public string ToString() => ""something"";
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_private_static_methods_sharing_same_name_and_1_method_in_between() => An_issue_is_reported_for(@"
public class TestMe
{
    private static int A() => 0815;

    private int B() => 4711;

    private static int A(object o) => 0815;
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_private_static_and_non_static_methods_sharing_same_name_and_non_static_methods_with_same_names_not_side_by_side_where_static_comes_first() => An_issue_is_reported_for(@"
public class TestMe
{
    private static int A() => 0815;

    private int B() => 4711;

    private int A(object o) => 0815;

    private int B(object o) => 0815;
}
");

        [Test]
        public void Code_gets_fixed_for_class_with_private_non_static_methods_sharing_same_name_and_not_side_by_side()
        {
            const string OriginalCode = @"
public class TestMe
{
    private static int A() => 0815;

    private int B() => 4711;

    private int C(object o) => 1;

    private int B(object o) => 2;
}
";

            const string FixedCode = @"
public class TestMe
{
    private static int A() => 0815;

    private int B() => 4711;

    private int B(object o) => 2;

    private int C(object o) => 1;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_private_static_methods_sharing_same_name_and_1_method_in_between()
        {
            const string OriginalCode = @"
public class TestMe
{
    private static int A() => 0815;

    private int B() => 4711;

    private static int A(object o) => 0815;
}
";

            const string FixedCode = @"
public class TestMe
{
    private static int A() => 0815;

    private static int A(object o) => 0815;

    private int B() => 4711;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_private_static_methods_and_non_static_methods_sharing_same_name_and_other_methods_in_between()
        {
            const string OriginalCode = @"
public class TestMe
{
    private static int A() => 1;

    private static int B(object o) => 3;

    private static int A(object o) => 2;

    private int C(object o) => 5;

    private int D() => 7;

    private int C() => 6;

    private static int B() => 4;
}
";

            const string FixedCode = @"
public class TestMe
{
    private static int A() => 1;

    private static int A(object o) => 2;

    private static int B(object o) => 3;

    private static int B() => 4;

    private int C(object o) => 5;

    private int C() => 6;

    private int D() => 7;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_private_non_static_commented_methods_sharing_same_name_and_not_side_by_side()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something with A.
    /// </summary>
    private static int A() => 0815;

    /// <summary>
    /// Does something with B.
    /// </summary>
    private int B() => 4711;

    /// <summary>
    /// Does something with C.
    /// </summary>
    private int C(object o) => 1;

    /// <summary>
    /// Does something else with B.
    /// </summary>
    private int B(object o) => 2;
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something with A.
    /// </summary>
    private static int A() => 0815;

    /// <summary>
    /// Does something with B.
    /// </summary>
    private int B() => 4711;

    /// <summary>
    /// Does something else with B.
    /// </summary>
    private int B(object o) => 2;

    /// <summary>
    /// Does something with C.
    /// </summary>
    private int C(object o) => 1;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4002_MethodsWithSameNameOrderedSideBySideAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4002_CodeFixProvider();
    }
}