using System.Collections.Generic;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1102_TestMethodsAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_test_method_with_correct_name(string testClassAttribute, string testAttribute) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_test_method_with_wrong_name(string testClassAttribute, string testAttribute) => An_issue_is_reported_for(@"
[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoTestSomething() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1102_TestMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1102_TestMethodsAnalyzer();

        private static IEnumerable<string> TestClassMarkers() => new[] { nameof(TestFixtureAttribute), "TestFixture", "TestClassAttribute", "TestClass" };

    }
}