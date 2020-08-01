using Microsoft.CodeAnalysis.CodeFixes;
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
        public void No_issue_is_reported_for_test_setup_method(
                                                    [ValueSource(nameof(TestFixtures))] string testFixture,
                                                    [ValueSource(nameof(TestSetUps))] string test)
            => No_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_teardown_method_with_correct_name(
                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                            [ValueSource(nameof(TestTearDowns))] string test)
            => No_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void CleanupTest() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_teardown_method_with_wrong_name(
                                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                            [ValueSource(nameof(TestTearDowns))] string test)
            => An_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void Teardown() { }
}
");

        [Test]
        public void Code_gets_fixed([ValueSource(nameof(TestTearDowns))] string test) => VerifyCSharpFix(
                                                                 @"using System; class TestMe { [" + test + @"] void Teardown()  { } }",
                                                                 @"using System; class TestMe { [" + test + @"] void CleanupTest()  { } }");

        protected override string GetDiagnosticId() => MiKo_1104_TestTeardownMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1104_TestTeardownMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1104_CodeFixProvider();
    }
}