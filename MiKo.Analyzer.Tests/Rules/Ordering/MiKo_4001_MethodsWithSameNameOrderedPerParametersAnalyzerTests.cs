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
        public void An_issue_is_reported_for_class_with_identically_named_methods_in_wrong_order() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    { }

    public void DoSomething()
    { }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_identically_named_methods_in_wrong_mixed_order() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_class_with_identically_named_methods_in_correct_order_and_params_method_in_the_middle() => An_issue_is_reported_for(@"
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

        protected override string GetDiagnosticId() => MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_4001_MethodsWithSameNameOrderedPerParametersAnalyzer();
    }
}