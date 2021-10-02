using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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

        protected override string GetDiagnosticId() => MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_4001_CodeFixProvider();
    }
}