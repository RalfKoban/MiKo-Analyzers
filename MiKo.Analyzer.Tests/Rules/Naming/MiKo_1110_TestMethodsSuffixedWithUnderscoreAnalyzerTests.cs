using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1110_TestMethodsSuffixedWithUnderscoreAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }

    public void DoSomething(int i) { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_parameterless_test_method_with_correct_name(
                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                        [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_parameterized_test_method_with_correct_name(
                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                        [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something_(int i) { }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_method_with_wrong_name(
                                                                    [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                    [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something(int i) { }
}
");

        [Test]
        public void Fix_can_be_made([ValueSource(nameof(Tests))] string test) => VerifyCSharpFix(
                                                                                         "class TestMe { [" + test + "] void DoSomething(int i) { } }",
                                                                                         "class TestMe { [" + test + "] void DoSomething_(int i) { } }");

        protected override string GetDiagnosticId() => MiKo_1110_TestMethodsSuffixedWithUnderscoreAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1110_TestMethodsSuffixedWithUnderscoreAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1110_CodeFixProvider();
    }
}