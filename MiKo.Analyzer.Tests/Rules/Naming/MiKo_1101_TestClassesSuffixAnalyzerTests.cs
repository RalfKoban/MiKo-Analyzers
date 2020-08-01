using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1101_TestClassesSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_test_class_with_correct_suffix_([ValueSource(nameof(TestFixtures))]string testFixture) => No_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMeTests
{
    public void DoSomething() { }
}
");

        [Test]
        public void An_issue_is_reported_for_test_class_with_wrong_suffix_([ValueSource(nameof(TestFixtures))] string testFixture) => An_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    private void DoSomething() { }
}
");

        [TestCase("[TestFixture] class TestMe { }", "[TestFixture] class TestMeTests { }")]
        [TestCase("[TestFixture] class TestMeTest { }", "[TestFixture] class TestMeTests { }")]
        public void Code_gets_fixed(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_1101_TestClassesSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1101_TestClassesSuffixAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1101_CodeFixProvider();
    }
}