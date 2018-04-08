using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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

        [TestCase(nameof(TestFixtureAttribute), nameof(TestAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseSourceAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TheoryAttribute))]
        [TestCase("TestFixture", "Test")]
        [TestCase("TestFixture", "TestCase")]
        [TestCase("TestFixture", "TestCaseSource")]
        [TestCase("TestFixture", "Theory")]
        [TestCase("TestClassAttribute", "TestMethodAttribute")]
        [TestCase("TestClass", "TestMethod")]
        public void No_issue_is_reported_for_test_class_with_tests(string testClassAttribute, string testAttribute) => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}
");

        [TestCase(nameof(TestFixtureAttribute), nameof(TestAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseSourceAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TheoryAttribute))]
        [TestCase("TestFixture", "Test")]
        [TestCase("TestFixture", "TestCase")]
        [TestCase("TestFixture", "TestCaseSource")]
        [TestCase("TestFixture", "Theory")]
        [TestCase("TestClassAttribute", "TestMethodAttribute")]
        [TestCase("TestClass", "TestMethod")]
        public void No_issue_is_reported_for_partial_test_class_with_tests(string testClassAttribute, string testAttribute) => No_issue_is_reported_for(@"
using NUnit.Framework;

public partial class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}

[" + testClassAttribute + @"]
public partial class TestMe
{
    private void DoSomethingElse() { }
}
");

        [TestCase(nameof(TestFixtureAttribute), nameof(TestAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseSourceAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TheoryAttribute))]
        [TestCase("TestFixture", "Test")]
        [TestCase("TestFixture", "TestCase")]
        [TestCase("TestFixture", "TestCaseSource")]
        [TestCase("TestFixture", "Theory")]
        [TestCase("TestClassAttribute", "TestMethodAttribute")]
        [TestCase("TestClass", "TestMethod")]
        public void No_issue_is_reported_for_test_class_with_multiple_base_classes_with_tests(string testClassAttribute, string testAttribute) => No_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe3 : TestMe2
{
    [" + testAttribute + @"]
    public void DoSomething3()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}

public class TestMe2 : TestMe1
{
    [" + testAttribute + @"]
    public void DoSomething2()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}

public class TestMe1
{
    [" + testAttribute + @"]
    public void DoSomething1()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}
");

        [TestCase(nameof(TestFixtureAttribute), nameof(TestAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseSourceAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TheoryAttribute))]
        [TestCase("TestFixture", "Test")]
        [TestCase("TestFixture", "TestCase")]
        [TestCase("TestFixture", "TestCaseSource")]
        [TestCase("TestFixture", "Theory")]
        [TestCase("TestClassAttribute", "TestMethodAttribute")]
        [TestCase("TestClass", "TestMethod")]
        public void An_issue_is_reported_for_test_class_with_tests_that_have_conditions(string testClassAttribute, string testAttribute) => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething()
    {
        if (x == 42)
            Assert.Fail(""42"");
    }
}
");

        [TestCase(nameof(TestFixtureAttribute), nameof(TestAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TestCaseSourceAttribute))]
        [TestCase(nameof(TestFixtureAttribute), nameof(TheoryAttribute))]
        [TestCase("TestFixture", "Test")]
        [TestCase("TestFixture", "TestCase")]
        [TestCase("TestFixture", "TestCaseSource")]
        [TestCase("TestFixture", "Theory")]
        [TestCase("TestClassAttribute", "TestMethodAttribute")]
        [TestCase("TestClass", "TestMethod")]
        public void An_issue_is_reported_for_test_class_with_multiple_base_classes_with_tests_that_have_conditions(string testClassAttribute, string testAttribute) => An_issue_is_reported_for(@"
using NUnit.Framework;

[" + testClassAttribute + @"]
public class TestMe3 : TestMe2
{
    [" + testAttribute + @"]
    public void DoSomething3()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}

public class TestMe2 : TestMe1
{
    [" + testAttribute + @"]
    public void DoSomething2()
    {
        Assert.That(42, Is.EqualTo(42));
    }
}

public class TestMe1
{
    [" + testAttribute + @"]
    public void DoSomething1()
    {
        if (x == 42)
            Assert.Fail(""42"");
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3102_TestMethodsHaveNoConditionsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3102_TestMethodsHaveNoConditionsAnalyzer();
    }
}