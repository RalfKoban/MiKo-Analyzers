using System;
using System.Linq;

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

        [TestCase("Method_SomethingIsReturned", "Method_returns_something")]
        [TestCase("Method_SomethingWasReturned", "Method_returns_something")]
        [TestCase("Method_SomethingGetsReturned", "Method_returns_something")]
        [TestCase("Method_SomethingGotReturned", "Method_returns_something")]
        [TestCase("Method_SomethingHasReturned", "Method_returns_something")]
        [TestCase("Method_SomethingHadReturned", "Method_returns_something")]
        [TestCase("Method_SomethingWillReturn", "Method_returns_something")]
        [TestCase("Method_SomethingWillReturns", "Method_returns_something")]
        [TestCase("Method_SomethingThrewException", "Method_something_throws_exception")]
        [TestCase("Method_SomethingWillBeUpdated", "Method_updates_something")]
        [TestCase("Method_SomethingWasUpdated", "Method_updates_something")]
        [TestCase("Method_SomethingWillNotBeAdded", "Method_does_not_add_something")]
        [TestCase("Method_WillNotShowWarningDialog", "Method_does_not_show_warning_dialog")]
        [TestCase("Method_UI_will_be_refreshed_after_some_time", "Method_refreshes_UI_after_some_time")]
        [TestCase("Method_returns_whatever_result_returned_from_Manager", "Method_returns_whatever_result_of_Manager")]
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