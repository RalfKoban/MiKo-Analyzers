using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1111_TestMethodsWithoutParametersNotSuffixedWithUnderscoreAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething_() { }

    public void DoSomething(int i) { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_parameterless_test_method_with_correct_name_(
                                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                      [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"

[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_parameterized_test_method_with_correct_name_(
                                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                      [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"

[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something_(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_inside_test_method() => No_issue_is_reported_for(@"
using NUnit;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething_does_something()
    {
        void DoSomething_(int j) { }
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_method_with_wrong_name_(
                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                      [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"

[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something_() { }
}
");

        [Test]
        public void Code_gets_fixed_([ValueSource(nameof(Tests))] string test) => VerifyCSharpFix(
                                                                                              "class TestMe { [" + test + "] public void DoSomething_() { } }",
                                                                                              "class TestMe { [" + test + "] public void DoSomething() { } }");

        protected override string GetDiagnosticId() => MiKo_1111_TestMethodsWithoutParametersNotSuffixedWithUnderscoreAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1111_TestMethodsWithoutParametersNotSuffixedWithUnderscoreAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1111_CodeFixProvider();
    }
}