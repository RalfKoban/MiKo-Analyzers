using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3101_TestClassesHaveTestsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
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
[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething() { }
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
public partial class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething() { }
}

[" + testClassAttribute + @"]
public partial class TestMe
{
    private void DoSomethingElse() { }
}
");

        [TestCase(nameof(TestFixtureAttribute))]
        [TestCase("TestFixture")]
        [TestCase("TestClassAttribute")]
        [TestCase("TestClass")]
        public void An_issue_is_reported_for_test_class_without_tests(string testClassAttribute) => An_issue_is_reported_for(@"
[" + testClassAttribute + @"]
public class TestMe
{
    private void DoSomethingElse() { }
}
");

        protected override string GetDiagnosticId() => MiKo_3101_TestClassesHaveTestsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3101_TestClassesHaveTestsAnalyzer();
    }
}