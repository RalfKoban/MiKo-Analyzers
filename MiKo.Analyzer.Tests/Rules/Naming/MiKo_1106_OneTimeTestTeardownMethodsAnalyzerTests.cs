using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                      [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_setup_method_(
                                                            [ValueSource(nameof(TestFixtures))] string fixture,
                                                            [ValueSource(nameof(TestSetUps))] string test)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_teardown_method_(
                                                               [ValueSource(nameof(TestFixtures))] string fixture,
                                                               [ValueSource(nameof(TestTearDowns))] string test)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_OneTimeSetUp_method_with_correct_name_(
                                                                                [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                [ValueSource(nameof(TestOneTimeSetUps))] string oneTimeSetUp)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + oneTimeSetUp + @"]
    public void DoSometing() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_OneTimeTearDown_method_with_correct_name_(
                                                                                   [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                   [ValueSource(nameof(TestOneTimeTearDowns))] string oneTimeTearDown)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + oneTimeTearDown + @"]
    public void CleanupTestEnvironment() { }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_inside_OneTimeTearDown_method() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_OneTimeTearDown_method_with_wrong_name_(
                                                                                 [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                 [ValueSource(nameof(TestOneTimeTearDowns))] string oneTimeTearDown)
            => An_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + oneTimeTearDown + @"]
    public void Teardown() { }
}
");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(TestOneTimeTearDowns))] string oneTimeTearDown) => VerifyCSharpFix(
                                                                                                                        "using System; class TestMe { [" + oneTimeTearDown + "] public void Teardown()  { } }",
                                                                                                                        "using System; class TestMe { [" + oneTimeTearDown + "] public void CleanupTestEnvironment()  { } }");

        protected override string GetDiagnosticId() => MiKo_1106_OneTimeTestTeardownMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1106_OneTimeTestTeardownMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1106_CodeFixProvider();
    }
}