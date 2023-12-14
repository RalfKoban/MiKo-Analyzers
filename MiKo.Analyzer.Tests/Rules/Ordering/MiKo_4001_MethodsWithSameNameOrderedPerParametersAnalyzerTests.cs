using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [TestFixture]
    public sealed class MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzerTests : CodeFixVerifier
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
    public void DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_2_differently_named_methods() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    { }

    public void DoSomethingElse()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_2_identically_named_methods_with_different_accessibilities() => No_issue_is_reported_for(@"
public class TestMe
{
    internal void DoSomething(int i)
    { }

    public void DoSomething()
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_identically_named_methods_in_correct_order() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_differently_named_methods_in_mixed_order() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomethingA()
    { }

    public void DoSomethingB(int i, int j)
    { }

    public void DoSomethingC(int i)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_identically_named_methods_in_different_accessibilities_and_mixed_order() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    { }

    protected void DoSomething(int i, int j)
    { }

    private void DoSomething(int i)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_ctors_in_different_accessibilities_and_mixed_order() => No_issue_is_reported_for(@"
public class TestMe
{
    public TestMe()
    { }

    protected TestMe(int i, int j)
    { }

    private TestMe(int i)
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_class_with_identically_named_methods_in_correct_order_and_params_method_at_the_end() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }

    public void DoSomething(params int[] i)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_2_identically_named_methods_in_wrong_order() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    { }

    public void DoSomething()
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_3_identically_named_methods_in_wrong_mixed_order() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    { }

    public void DoSomething(int i, int j)
    { }

    public void DoSomething(int i)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_4_identically_named_methods_in_correct_order_and_params_method_in_the_middle() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    { }

    public void DoSomething(params int[] i)
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_ctors_in_mixed_order() => An_issue_is_reported_for(@"
public class TestMe
{
    public TestMe()
    { }

    public TestMe(int i, int j)
    { }

    public TestMe(int i)
    { }
}
");

        [Test]
        public void Code_gets_fixed_for_class_with_2_identically_named_methods_in_wrong_order()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    { }

    public void DoSomething()
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_3_identically_named_methods_in_wrong_mixed_order()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    { }

    public void DoSomething(int i, int j)
    { }

    public void DoSomething(int i)
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_4_identically_named_methods_in_correct_order_and_params_method_in_the_middle()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    { }

    public void DoSomething(params int[] i)
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }

    public void DoSomething(params int[] i)
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_4_identically_named_methods_in_mixed_order_and_other_methods_in_between()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(params int[] i)
    { }

    public void DoSomething(int i)
    { }

    public void DoAnything()
    {
    }

    public void DoSomething()
    { }

    public void DoSomething(int i, int j)
    { }

    public void DoAnythingElse()
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoAnything()
    {
    }

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }

    public void DoSomething(params int[] i)
    { }

    public void DoAnythingElse()
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_2_ctors_in_wrong_order()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public TestMe(int i, int j)
        { }

        public TestMe(int i)
        { }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public TestMe(int i)
        { }

        public TestMe(int i, int j)
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_3_ctors_in_wrong_order()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public TestMe(int i, int j)
        { }

        public TestMe(int i)
        { }

        public TestMe()
        { }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public TestMe()
        { }

        public TestMe(int i)
        { }

        public TestMe(int i, int j)
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_3_ctors_in_mixed_order()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public TestMe()
        { }

        public TestMe(int i, int j)
        { }

        public TestMe(int i)
        { }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public TestMe()
        { }

        public TestMe(int i)
        { }

        public TestMe(int i, int j)
        { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_identically_named_methods_in_different_accessibilities_and_mixed_order_and_other_methods_in_between()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(params int[] i)
    { }

    public void DoSomething(int i)
    { }

    public static void DoSomething(int i, object i)
    { }

    public static void DoSomething()
    { }

    public void DoAnything()
    { }

    public void DoSomething(int i, int j)
    { }

    public static void DoSomething(object o)
    { }

    public void DoAnythingElse()
    { }

    private void DoAnythingElse(int i)
    { }

    private void DoSomething(int i, string s)
    { }

    private static void DoSomething(int i, string s, object o)
    { }

    private static void DoSomething(string s)
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    public static void DoSomething()
    { }

    public static void DoSomething(object o)
    { }

    public static void DoSomething(int i, object i)
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }

    public void DoSomething(params int[] i)
    { }

    public void DoAnything()
    { }

    public void DoAnythingElse()
    { }

    private void DoAnythingElse(int i)
    { }

    private static void DoSomething(string s)
    { }

    private static void DoSomething(int i, string s, object o)
    { }

    private void DoSomething(int i, string s)
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_2_identically_named_commented_methods_in_wrong_order()
        {
            const string OriginalCode = @"
public class TestMe
{
    /// <summary>
    /// Does something with integers.
    /// </summary>
    public void DoSomething(int i)
    { }

    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething()
    { }
}
";

            const string FixedCode = @"
public class TestMe
{
    /// <summary>
    /// Does something.
    /// </summary>
    public void DoSomething()
    { }

    /// <summary>
    /// Does something with integers.
    /// </summary>
    public void DoSomething(int i)
    { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_ctors_on_structs_that_do_not_have_a_parameterless_ctor()
        {
            const string OriginalCode = @"
public readonly struct TestMe
{
    public TestMe(string s1, string s2) { }

    public TestMe(string s) { }
}
";

            const string FixedCode = @"
public readonly struct TestMe
{
    public TestMe(string s) { }

    public TestMe(string s1, string s2) { }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_2_identically_named_methods_in_wrong_order_within_region()
        {
            const string OriginalCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething(int i)
    { }

    public void DoSomething()
    { }

    #endregion
}
";

            const string FixedCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_3_identically_named_methods_in_wrong_mixed_order_within_region()
        {
            const string OriginalCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething(int i, int j)
    { }

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    #endregion
}
";

            const string FixedCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_4_identically_named_methods_in_correct_order_and_params_method_at_beginning_within_region()
        {
            const string OriginalCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething(params int[] i)
    { }

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }

    #endregion
}
";

            const string FixedCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }

    public void DoSomething(params int[] i)
    { }

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_uncommented_methods_within_region_after_constructors()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe()
    { }

    #region Public members

    public void DoSomething(int i)
    { }

    public void DoSomething()
    { }

    #endregion

    #region Private members

    private void DoAnything()
    { }

    #endregion
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe()
    { }

    #region Public members

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    #endregion

    #region Private members

    private void DoAnything()
    { }

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_commented_methods_within_region_after_constructors()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe()
    { }

    #region Public members

    /// <summary>Comment #1</summary>
    public void DoSomething(int i)
    { }

    /// <summary>Comment #2</summary>
    public void DoSomething()
    { }

    #endregion

    #region Private members

    private void DoAnything()
    { }

    #endregion
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe()
    { }

    #region Public members

    /// <summary>Comment #2</summary>
    public void DoSomething()
    { }

    /// <summary>Comment #1</summary>
    public void DoSomething(int i)
    { }

    #endregion

    #region Private members

    private void DoAnything()
    { }

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_uncommented_methods_within_region_if_another_method_is_between_them()
        {
            const string OriginalCode = @"
public class TestMe
{
    public TestMe()
    { }

    #region Public members

    public void DoSomething(int i)
    { }

    public void DoAnything(int i)
    { }

    public void DoSomething()
    { }

    #endregion

    #region Private members

    private void DoAnything()
    { }

    #endregion
}
";

            const string FixedCode = @"
public class TestMe
{
    public TestMe()
    { }

    #region Public members

    public void DoAnything(int i)
    { }

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    #endregion

    #region Private members

    private void DoAnything()
    { }

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_3_uncommented_methods_within_region_if_method_with_most_parameters_is_1st_and_inside_own_region()
        {
            const string OriginalCode = @"
public class TestMe
{
    #region Public members

    #region Special region

    public void DoSomething(int i, int j)
    { }

    #endregion

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    #endregion
}
";

            const string FixedCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    #region Special region

    public void DoSomething(int i, int j)
    { }

    #endregion

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_3_uncommented_methods_within_region_if_method_with_most_parameters_is_2nd_and_inside_own_region()
        {
            const string OriginalCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething(int i)
    { }

    #region Special region

    public void DoSomething(int i, int j)
    { }

    #endregion

    public void DoSomething()
    { }

    #endregion
}
";

            const string FixedCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    #region Special region

    public void DoSomething(int i, int j)
    { }

    #endregion

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_4_uncommented_methods_within_region_if_method_with_most_parameters_is_3rd_and_inside_own_region()
        {
            const string OriginalCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething(int i, int j)
    { }

    public void DoSomething(int i)
    { }

    #region Special region

    public void DoSomething(int i, int j, int k)
    { }

    #endregion

    public void DoSomething()
    { }

    #endregion
}
";

            const string FixedCode = @"
public class TestMe
{
    #region Public members

    public void DoSomething()
    { }

    public void DoSomething(int i)
    { }

    public void DoSomething(int i, int j)
    { }

    #region Special region

    public void DoSomething(int i, int j, int k)
    { }

    #endregion

    #endregion
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4001_CodeFixProvider();
    }
}