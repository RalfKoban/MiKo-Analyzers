using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3133_AvoidTrivialAssertionHelpersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_test_as_expression_body_([ValueSource(nameof(Tests))] string testAttribute) => No_issue_is_reported_for(@"

public class TestMe
{
    [" + testAttribute + @"]
    public void TestSomething() => Assert.That(true, Is.True);
}
");

        [Test]
        public void No_issue_is_reported_for_test_as_body_([ValueSource(nameof(Tests))] string testAttribute) => No_issue_is_reported_for(@"

public class TestMe
{
    [" + testAttribute + @"]
    public void TestSomething()
    {
        Assert.That(true, Is.True);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_trivial_test_helper_as_body() => No_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper(int i, int j)
    {
        Assert.That(i, Is.EqualTo(47));
        Assert.That(j, Is.EqualTo(47));
    }
}
");

        [Test] // this is still a trivial helper but for now we do not report such
        public void No_issue_is_reported_for_non_trivial_test_helper_with_const_variable_as_body() => No_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper(int i)
    {
        const int x = 42;

        Assert.That(i, Is.EqualTo(x));
    }
}
");

        [Test] // this is still a trivial helper but for now we do not report such
        public void No_issue_is_reported_for_non_trivial_test_helper_with_variable_as_body() => No_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper(int i)
    {
        int x = 42;

        Assert.That(i, Is.EqualTo(x));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_trivial_test_helper_as_body() => An_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper(int i)
    {
        Assert.That(i, Is.EqualTo(42));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_trivial_test_helper_as_expression_body() => An_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper(int i) => Assert.That(i, Is.EqualTo(42));
}
");

        protected override string GetDiagnosticId() => MiKo_3133_AvoidTrivialAssertionHelpersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3133_AvoidTrivialAssertionHelpersAnalyzer();
    }
}