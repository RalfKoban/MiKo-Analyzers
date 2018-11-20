using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1104_TestTeardownMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method(
                                                    [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                    [ValueSource(nameof(TestsExceptTearDowns))] string testAttribute)
            => No_issue_is_reported_for(@"
[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_cleanup_method_with_correct_name(
                                                                            [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                            [ValueSource(nameof(TestTearDowns))] string testAttribute)
            => No_issue_is_reported_for(@"
[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void CleanupTest() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_cleanup_method_with_wrong_name(
                                                                            [ValueSource(nameof(TestFixtures))] string testClassAttribute,
                                                                            [ValueSource(nameof(TestTearDowns))] string testAttribute)
            => An_issue_is_reported_for(@"
[" + testClassAttribute + @"]
public class TestMe
{
    [" + testAttribute + @"]
    public void Teardown() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1104_TestTeardownMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1104_TestTeardownMethodsAnalyzer();
    }
}