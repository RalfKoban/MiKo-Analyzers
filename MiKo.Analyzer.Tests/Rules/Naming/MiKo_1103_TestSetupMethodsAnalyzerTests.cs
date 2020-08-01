using Microsoft.CodeAnalysis.CodeFixes;
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

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method(
                                                    [ValueSource(nameof(TestFixtures))] string testFixture,
                                                    [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_teardown_method(
                                                    [ValueSource(nameof(TestFixtures))] string testFixture,
                                                    [ValueSource(nameof(TestTearDowns))] string test)
            => No_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_setup_method_with_correct_name(
                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                            [ValueSource(nameof(TestSetUps))] string test)
            => No_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void PrepareTest() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_setup_method_with_wrong_name(
                                                                    [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                    [ValueSource(nameof(TestSetUps))] string test)
            => An_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void Setup() { }
}
");

        [Test]
        public void Fix_can_be_made([ValueSource(nameof(TestSetUps))] string test) => VerifyCSharpFix(
                                                              @"using System; class TestMe { [" + test + @"] void Setup()  { } }",
                                                              @"using System; class TestMe { [" + test + @"] void PrepareTest()  { } }");

        protected override string GetDiagnosticId() => MiKo_1103_TestSetupMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1103_TestSetupMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1103_CodeFixProvider();
    }
}