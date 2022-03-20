using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1107_TestMethodsPascalCasingAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method_with_correct_name_(
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
        public void No_issue_is_reported_for_test_method_with_correct_upper_and_lower_case_name_(
                                                                        [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                        [ValueSource(nameof(Tests))] string test)
            => No_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something_with_MyEvent_stuff() { }
}
");

        [Test]
        public void No_issue_is_reported_for_local_function_inside_test_method() => No_issue_is_reported_for(@"
using NUnit;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething_does_something_with_MyEvent_stuff()
    {
        void DoSomethingMyEventCore() { }
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_test_method_with_wrong_name_(
                                                                    [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                    [ValueSource(nameof(Tests))] string test,
                                                                    [Values("DoSomethingDoesSomething", "DoSomething_DoesSomething", "DoSomething_Expect_DoSomething")] string name)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void " + name + @"() { }
}
");

        [TestCase("DoSomething_doesSomething", "Do_something_does_something")]
        [TestCase("DoSomething_DoesSomething", "Do_something_does_something")]
        [TestCase("doSomethingDoesSomething", "do_something_does_something")]
        [TestCase("DoSomethingDoesSomething", "Do_something_does_something")]
        [TestCase("DoSomethingMustBeSomething", "Do_something_is_something")]
        [TestCase("DoSomethingMustNotBeSomething", "Do_something_is_not_something")]
        [TestCase("DoSomethingShallBeSomething", "Do_something_is_something")]
        [TestCase("DoSomethingShallNotBeSomething", "Do_something_is_not_something")]
        [TestCase("DoSomethingShouldBeSomething", "Do_something_is_something")]
        [TestCase("DoSomethingShouldNotBeSomething", "Do_something_is_not_something")]
        [TestCase("DoSomethingWithHTML", "Do_something_with_HTML")]
        [TestCase("DoSomethingWithHTMLandMore", "Do_something_with_HTML_and_more")]
        [TestCase("DoSomethingX", "Do_something_X")]
        [TestCase("HTMLdoSomething", "HTML_do_something")]
        [TestCase("ShowsAWarning", "Shows_a_warning")]
        [TestCase("ThrowsArgumentExceptionForSomething", "Throws_ArgumentException_for_something")]
        [TestCase("ThrowsArgumentNullExceptionForSomething", "Throws_ArgumentNullException_for_something")]
        [TestCase("ThrowsArgumentOutOfRangeExceptionForSomething", "Throws_ArgumentOutOfRangeException_for_something")]
        [TestCase("ThrowsInvalidOperationExceptionForSomething", "Throws_InvalidOperationException_for_something")]
        [TestCase("ThrowsNotImplementedExceptionForSomething", "Throws_NotImplementedException_for_something")]
        [TestCase("ThrowsNotSupportedExceptionForSomething", "Throws_NotSupportedException_for_something")]
        [TestCase("ThrowsObjectDisposedExceptionForSomething", "Throws_ObjectDisposedException_for_something")]
        [TestCase("ThrowsOperationCanceledExceptionForSomething", "Throws_OperationCanceledException_for_something")]
        [TestCase("ThrowsTaskCanceledExceptionForSomething", "Throws_TaskCanceledException_for_something")]
        public void Code_gets_fixed_for_test_method_(string original, string fix)
        {
            const string Template = @"
[TestFixture]
public class TestMe
{
    [Test]
    public void ###() { }
}
";

            VerifyCSharpFix(Template.Replace("###", original), Template.Replace("###", fix));
        }

        protected override string GetDiagnosticId() => MiKo_1107_TestMethodsPascalCasingAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1107_TestMethodsPascalCasingAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1107_CodeFixProvider();
    }
}