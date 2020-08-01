using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1105_OneTimeTestSetupMethodsAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_one_time_test_teardown_method_(
                                                            [ValueSource(nameof(TestFixtures))] string testFixture,
                                                            [ValueSource(nameof(TestOneTimeTearDowns))] string test)
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
    public void PrepareTestEnvironment() { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_one_time_test_setup_method_with_wrong_name_(
                                                                                [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                                [ValueSource(nameof(TestOneTimeSetUps))] string test)
            => An_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void Setup() { }
}
");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(TestOneTimeSetUps))] string test) => VerifyCSharpFix(
                                                                 @"using System; class TestMe { [" + test + @"] void Setup()  { } }",
                                                                 @"using System; class TestMe { [" + test + @"] void PrepareTestEnvironment()  { } }");

        protected override string GetDiagnosticId() => MiKo_1105_OneTimeTestSetupMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1105_OneTimeTestSetupMethodsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1105_CodeFixProvider();
    }
}