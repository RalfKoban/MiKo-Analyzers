using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1103_TestSetupMethodsAnalyzerTests : CodeFixVerifier
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
        [TestCase(nameof(TestFixtureAttribute), nameof(TearDownAttribute))]
        [TestCase("TestFixture", "TearDown")]
        [TestCase("TestClassAttribute", "TestCleanupAttribute")]
        [TestCase("TestClass", "TestCleanup")]
        public void No_issue_is_reported_for_test_method(string testClassAttribute, string testAttribute) => No_issue_is_reported_for(@"
[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething() { }
}
");

        [TestCase(nameof(TestFixtureAttribute), nameof(SetUpAttribute))]
        [TestCase("TestFixture", "SetUp")]
        [TestCase("TestClassAttribute", "TestInitializeAttribute")]
        [TestCase("TestClass", "TestInitialize")]
        public void No_issue_is_reported_for_test_setup_method_with_correct_name(string testClassAttribute, string testAttribute) => No_issue_is_reported_for(@"
[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void PrepareTest() { }
}
");

        [TestCase(nameof(TestFixtureAttribute), nameof(SetUpAttribute))]
        [TestCase("TestFixture", "SetUp")]
        [TestCase("TestClassAttribute", "TestInitializeAttribute")]
        [TestCase("TestClass", "TestInitialize")]
        public void An_issue_is_reported_for_test_setup_method_with_wrong_name(string testClassAttribute, string testAttribute) => An_issue_is_reported_for(@"
[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void Setup() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1103_TestSetupMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1103_TestSetupMethodsAnalyzer();
    }
}