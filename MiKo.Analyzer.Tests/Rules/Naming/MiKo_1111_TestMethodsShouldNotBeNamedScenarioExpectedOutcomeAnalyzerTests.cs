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
                "DoSomething_WithUnnecessaryDataForSomething",
            };

        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }

    public void DoSomething(int i) { }
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
        public void An_issue_is_reported_for_test_method_with_wrong_name_(
                                                                    [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                    [ValueSource(nameof(Tests))] string test,
                                                                    [ValueSource(nameof(WrongMethodNames))] string methodName)
            => An_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void " + methodName + @"() { }
}
");

        [TestCase("DoesSomething_IsExceptional", "Throws_exception_if_does_something")]
        [TestCase("NoDocumentIsDirty_ProjectIsNotDirty", "Project_is_not_dirty_if_no_document_is_dirty")]
        [TestCase("IfLoadFails_ReturnsNull", "Returns_null_if_load_fails")]
        public void Code_gets_fixed_(string originalName, string fixedName) => VerifyCSharpFix(
                                                                                         "class TestMe { [Test] void " + originalName + "() { } }",
                                                                                         "class TestMe { [Test] void " + fixedName + "() { } }");

        protected override string GetDiagnosticId() => MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1111_CodeFixProvider();
    }
}