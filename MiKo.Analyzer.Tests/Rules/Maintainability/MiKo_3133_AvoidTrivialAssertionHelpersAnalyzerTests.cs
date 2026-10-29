using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3133_AvoidTrivialAssertionHelpersAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] TestRelatedAttributes = Enumerable.Empty<string>()
                                                                           .Concat(Tests)
                                                                           .Concat(TestSetUps)
                                                                           .Concat(TestTearDowns)
                                                                           .Concat(TestOneTimeSetUps)
                                                                           .Concat(TestOneTimeTearDowns)
                                                                           .Concat(TestAssemblySetUps)
                                                                           .Concat(TestAssemblyTearDowns)
                                                                           .ToArray();

        [Test]
        public void No_issue_is_reported_for_test_as_expression_body_([ValueSource(nameof(TestRelatedAttributes))] string testAttribute) => No_issue_is_reported_for(@"

public class TestMe
{
    [" + testAttribute + @"]
    public void TestSomething() => Assert.That(true, Is.True);
}
");

        [Test]
        public void No_issue_is_reported_for_test_as_body_([ValueSource(nameof(TestRelatedAttributes))] string testAttribute) => No_issue_is_reported_for(@"

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
        public void No_issue_is_reported_for_helper_using_Assert_Multiple_as_body() => No_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper(bool a, bool b)
    {
        Assert.Multiple(() =>
                             {
                                 Assert.That(a, Is.True);
                                 Assert.That(b, Is.True);
                             });
    }
}
");

        [Test]
        public void No_issue_is_reported_for_helper_using_Assert_Multiple_as_expression_body() => No_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper(bool a, bool b) => Assert.Multiple(() =>
                                                                      {
                                                                          Assert.That(a, Is.True);
                                                                          Assert.That(b, Is.True);
                                                                      });
}
");

        [Test]
        public void No_issue_is_reported_for_helper_using_Assert_MultipleAsync_as_body() => No_issue_is_reported_for(@"

using System.Threading.Tasks;

public class TestMe
{
    public void AssertHelper(bool a)
    {
        Assert.MultipleAsync(() =>
                                  {
                                      Assert.That(a, Is.True);
                                      return Task.CompletedTask;
                                  });
    }
}
");

        [Test]
        public void No_issue_is_reported_for_helper_using_Assert_MultipleAsync_as_expression_body() => No_issue_is_reported_for(@"

using System.Threading.Tasks;

public class TestMe
{
    public void AssertHelper(bool a) => Assert.MultipleAsync(() =>
                                                                  {
                                                                      Assert.That(a, Is.True);
                                                                      return Task.CompletedTask;
                                                                  });
}
");

        [Test]
        public void No_issue_is_reported_for_helper_using_Assert_EnterMultipleScope_as_body() => No_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper()
    {
        Assert.EnterMultipleScope();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_helper_using_Assert_EnterMultipleScope_as_expression_body() => No_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper() => Assert.EnterMultipleScope();
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

        [Test]
        public void An_issue_is_reported_for_trivial_test_helper_as_body_using_fully_qualified_name() => An_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper(int i)
    {
        NUnit.Framework.Assert.That(i, Is.EqualTo(42));
    }
}
");

        [Test]
        public void An_issue_is_reported_for_trivial_test_helper_as_expression_body_fully_qualified_name() => An_issue_is_reported_for(@"

public class TestMe
{
    public void AssertHelper(int i) => NUnit.Framework.Assert.That(i, Is.EqualTo(42));
}
");

        [Test]
        public void An_issue_is_reported_for_trivial_awaited_test_helper_as_body() => An_issue_is_reported_for(@"

using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task AssertHelper(AsyncTestDelegate action)
    {
        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_trivial_awaited_test_helper_as_expression_body() => An_issue_is_reported_for(@"

using System;
using System.Threading.Tasks;

public class TestMe
{
    public async Task AssertHelper(AsyncTestDelegate action) => await Assert.ThrowsAsync<InvalidOperationException>(action);
}
");

        protected override string GetDiagnosticId() => MiKo_3133_AvoidTrivialAssertionHelpersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3133_AvoidTrivialAssertionHelpersAnalyzer();
    }
}