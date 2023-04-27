using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongMethodNames =
            {
                "MethodName_Scenario_ExpectedOutcome",
                "DoSomething_WithUnnecessaryData_ReturnsTrue",
                "DoSomething_WithUnnecessaryDataForSomething_ThrowsException",
            };

        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }

    public void DoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_correct_name()
            => Assert.Multiple(() =>
                                   {
                                       foreach (var testFixture in TestFixtures)
                                       {
                                           foreach (var test in Tests)
                                           {
                                               No_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething_does_something() { }
}
");
                                           }
                                       }
                                   });

        [Test]
        public void An_issue_is_reported_for_test_method_with_wrong_name_([ValueSource(nameof(WrongMethodNames))] string methodName)
            => Assert.Multiple(() =>
                                   {
                                       foreach (var testFixture in TestFixtures)
                                       {
                                           foreach (var test in Tests)
                                           {
                                               An_issue_is_reported_for(@"
[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void " + methodName + @"() { }
}
");
                                           }
                                       }
                                   });

        [TestCase("DoesSomething_IsExceptional", "Throws_exception_if_does_something")]
        [TestCase("NoDocumentIsDirty_ProjectIsNotDirty", "Project_is_not_dirty_if_no_document_is_dirty")]
        [TestCase("IfLoadFails_ReturnsNull", "Returns_null_if_load_fails")]
        [TestCase("MethodName_LoadFails_ReturnNull", "Method_name_returns_null_if_load_fails")]
        [TestCase("MethodName_SomeCondition_LoadFails_ReturnNull", "Method_name_returns_null_if_load_fails_and_some_condition")]
        [TestCase("Initialize_NothingCanBeFound_ShouldNotDoAnything", "Initialize_should_not_do_anything_if_nothing_can_be_found")]
        [TestCase("MethodName_ShouldDoSomething_And_SomethingMore", "Method_name_should_do_something_and_something_more")]
        [TestCase("MethodName_WhenSome_Stuff_ReturnsTrue", "Method_name_returns_true_when_some_stuff")]
        public void Code_gets_fixed_(string originalName, string fixedName) => VerifyCSharpFix(
                                                                                         "class TestMe { [Test] void " + originalName + "() { } }",
                                                                                         "class TestMe { [Test] void " + fixedName + "() { } }");

        protected override string GetDiagnosticId() => MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1111_CodeFixProvider();
    }
}