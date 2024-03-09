using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3102_TestMethodsHaveNoConditionsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_class_with_conditions_in_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int x)
    {
        if (x is null)
        {
        }

        switch (x)
        {
        }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_class_with_tests_(
                                                                [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_partial_test_class_with_tests_(
                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                        [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

public partial class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}

[" + testFixture + @"]
public partial class TestMe
{
    private void DoSomethingElse() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_class_with_multiple_base_classes_with_tests_(
                                                                                           [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                           [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe3 : TestMe2
{
    [" + test + @"]
    public void DoSomething3()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}

public class TestMe2 : TestMe1
{
    [" + test + @"]
    public void DoSomething2()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}

public class TestMe1
{
    [" + test + @"]
    public void DoSomething1()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_class_with_tests_that_have_conditions_(
                                                                                     [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                     [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
        if (x == 42)
            Assert.Fail(""42"");
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_class_with_multiple_base_classes_with_tests_that_have_conditions_(
                                                                                                                [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                                                [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe3 : TestMe2
{
    [" + test + @"]
    public void DoSomething3()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}

public class TestMe2 : TestMe1
{
    [" + test + @"]
    public void DoSomething2()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}

public class TestMe1
{
    [" + test + @"]
    public void DoSomething1()
    {
        if (x == 42)
            Assert.Fail(""42"");
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_class_with_tests_that_have_a_coalesce_condition_(
                                                                                               [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                               [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething(object o1, object o2)
    {
        var x = o1 ?? o2;

        Assert.That(x, Is.Not.Null);
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_class_with_tests_that_have_a_coalesce_assignment_condition_(
                                                                                                          [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                                          [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething(object o1, object o2)
    {
            o1 ??= o2;

            Assert.That(o1, Is.Not.Null);
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_Nullable_object_creation_(
                                                                   [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                   [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
using NUnit.Framework;

using System.Threading;

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething()
    {
        var token = new CancellationToken?();
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3102_TestMethodsHaveNoConditionsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3102_TestMethodsHaveNoConditionsAnalyzer();
    }
}