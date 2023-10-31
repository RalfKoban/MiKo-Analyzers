using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1106_OneTimeTestTeardownMethodsAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method_(
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
        public void No_issue_is_reported_for_test_setup_method_(
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
        public void No_issue_is_reported_for_test_teardown_method_(
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
        public void No_issue_is_reported_for_one_time_test_setup_method_with_correct_name_(
                                                                                       [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                       [ValueSource(nameof(TestOneTimeSetUps))] string test)
            => No_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSometing() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_one_time_test_teardown_method_with_correct_name_(
                                                                                          [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                          [ValueSource(nameof(TestOneTimeTearDowns))] string test)
            => No_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void CleanupTestEnvironment() { }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_inside_one_time_test_teardown_method() => No_issue_is_reported_for(@"
using NUnit;

[TestFixture]
public class TestMe
{
    [OneTimeTearDown]
    public void CleanupTestEnvironment()
    {
        void Teardown() { }
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_one_time_test_teardown_method_with_wrong_name_(
                                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                        [ValueSource(nameof(TestOneTimeTearDowns))] string test)
            => An_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void Teardown() { }
}
");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(TestOneTimeTearDowns))] string test) => VerifyCSharpFix(
                                                                "using System; class TestMe { [" + test + "] public void Teardown()  { } }",
                                                                "using System; class TestMe { [" + test + "] public void CleanupTestEnvironment()  { } }");

        protected override string GetDiagnosticId() => MiKo_1106_OneTimeTestTeardownMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1106_OneTimeTestTeardownMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1106_CodeFixProvider();
    }
}