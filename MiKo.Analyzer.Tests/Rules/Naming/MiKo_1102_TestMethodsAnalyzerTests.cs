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

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method_with_correct_name(
                                                                    [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                    [ValueSource(nameof(Tests))] string testAttribute)
            => No_issue_is_reported_for(@"

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_method_with_wrong_name(
                                                                    [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                    [ValueSource(nameof(TestsExceptSetUpTearDowns))] string testAttribute)
            => An_issue_is_reported_for(@"

[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoTestSomething() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1102_TestMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1102_TestMethodsAnalyzer();
    }
}