using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1116_TestMethodsShouldBeInPresentTenseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] NonPresentPhrases = ["Was", "Returned", "Will", "Threw", "Got", "Had"];

        [Test]
        public void No_issue_is_reported_for_non_test_method_with_present_tense() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_method_with_([ValueSource(nameof(NonPresentPhrases))] string phrase) => No_issue_is_reported_for(@"
public class TestMe
{
    public int " + phrase + @"Something() => 42;
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_present_tense() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public int DoSomething() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_([ValueSource(nameof(NonPresentPhrases))] string phrase) => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public int " + phrase + @"Something() => 42;
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_lower_case_([ValueSource(nameof(NonPresentPhrases))] string phrase) => An_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public int Method_" + phrase.ToLowerCaseAt(0) + @"_something() => 42;
}
");

        [TestCase("Method_Something_IsReturned", "Method_returns_something")]
        [TestCase("Method_Something_WasReturned", "Method_returns_something")]
        [TestCase("Method_Something_GetsReturned", "Method_returns_something")]
        [TestCase("Method_Something_GotReturned", "Method_returns_something")]
        [TestCase("Method_Something_HasReturned", "Method_returns_something")]
        [TestCase("Method_Something_HadReturned", "Method_returns_something")]
        [TestCase("Method_Something_WillReturn", "Method_returns_something")]
        [TestCase("Method_Something_WillReturns", "Method_returns_something")]
        [TestCase("Method_Something_WillBeUpdated", "Method_is_updated_if_something")]
        [TestCase("Method_Something_WasUpdated", "Method_is_updated_if_something")]
        [TestCase("Method_Something_WillNotBeAdded", "Method_does_not_add_if_something")]
        [TestCase("Method_Something_ThrewException", "Method_throws_exception_if_something")]
        [TestCase("Method_WillNotShowWarningDialog", "Method_does_not_show_warning_dialog")]
        [TestCase("Method_WontShowWarningDialog", "Method_does_not_show_warning_dialog")]
        [TestCase("Method_UI_will_be_refreshed_after_some_time", "Method_UI_is_refreshed_after_some_time")]
        [TestCase("Method_returns_whatever_result_returned_from_manager", "Method_returns_whatever_result_of_manager")]
        [TestCase("TestThatCorrectResourceExistsForConfigCallThrewException", "Correct_resource_exists_for_config_call_throws_exception")]
        [TestCase("TestIfCorrectResourceExistsForConfigCallThrewException", "Correct_resource_exists_for_config_call_throws_exception")]
        [TestCase("TestWhetherCorrectResourceExistsForConfigCallThrewException", "Correct_resource_exists_for_config_call_throws_exception")]
        public void Code_gets_fixed_for_(string originalName, string fixedName)
        {
            const string Template = @"
using NUnit.Framework;

public class TestMe
{
    [Test]
    public int ###() => 42;
}
";

            VerifyCSharpFix(Template.Replace("###", originalName), Template.Replace("###", fixedName));
        }

        protected override string GetDiagnosticId() => MiKo_1116_TestMethodsShouldBeInPresentTenseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1116_TestMethodsShouldBeInPresentTenseAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1116_CodeFixProvider();
    }
}