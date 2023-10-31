using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

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

        [TestCase("DoSomething_does_something")]
        [TestCase("Returns_null_if_load_fails")]
        [TestCase("Throws_ArgumentNullException_under_some_conditions")]
        [TestCase("Method_name_returns_false_if_load_fails_and_some_condition")]
        public void No_issue_is_reported_for_test_method_with_correct_name_(string methodName)
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
    public void " + methodName + @"() { }
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
        [TestCase("IfLoadFails_ReturnNull", "Returns_null_if_load_fails")]
        [TestCase("IfLoadFails_ReturnsNull", "Returns_null_if_load_fails")]
        [TestCase("NoDocumentIsDirty_ProjectIsNotDirty", "Project_is_not_dirty_if_no_document_is_dirty")]
        [TestCase("SomeCondition_ArgumentExceptionThrown", "Throws_ArgumentException_if_some_condition")]
        [TestCase("SomeCondition_ArgumentNullExceptionThrown", "Throws_ArgumentNullException_if_some_condition")]
        [TestCase("SomeCondition_ArgumentOutOfRangeExceptionThrown", "Throws_ArgumentOutOfRangeException_if_some_condition")]
        [TestCase("SomeCondition_ExceptionThrown", "Throws_exception_if_some_condition")]
        [TestCase("SomeCondition_FalseReturned", "Returns_false_if_some_condition")]
        [TestCase("SomeCondition_InvalidOperationExceptionThrown", "Throws_InvalidOperationException_if_some_condition")]
        [TestCase("SomeCondition_NotImplementedExceptionThrown", "Throws_NotImplementedException_if_some_condition")]
        [TestCase("SomeCondition_NotSupportedExceptionThrown", "Throws_NotSupportedException_if_some_condition")]
        [TestCase("SomeCondition_NullReturned", "Returns_null_if_some_condition")]
        [TestCase("SomeCondition_ObjectDisposedExceptionThrown", "Throws_ObjectDisposedException_if_some_condition")]
        [TestCase("SomeCondition_OperationCanceledExceptionThrown", "Throws_OperationCanceledException_if_some_condition")]
        [TestCase("SomeCondition_TaskCanceledExceptionThrown", "Throws_TaskCanceledException_if_some_condition")]
        [TestCase("SomeCondition_ThrowsArgumentException", "Throws_ArgumentException_if_some_condition")]
        [TestCase("SomeCondition_ThrowsArgumentNullException", "Throws_ArgumentNullException_if_some_condition")]
        [TestCase("SomeCondition_ThrowsException", "Throws_exception_if_some_condition")]
        [TestCase("SomeCondition_TrueReturned", "Returns_true_if_some_condition")]
        [TestCase("SomeCondition_Returned", "Returns_some_condition")]
        [TestCase("SomeCondition_ObjectReturned", "Returns_object_if_some_condition")]
        [TestCase("Method_ThrowsExceptionIfNull", "Method_throws_exception_if_null")]
        public void Code_gets_fixed_for_1_slash_in_(string originalName, string fixedName) => VerifyCSharpFix(
                                                                                                         "class TestMe { [Test] public void " + originalName + "() { } }",
                                                                                                         "class TestMe { [Test] public void " + fixedName + "() { } }");

        [TestCase("Initialize_NothingCanBeFound_ShouldNotDoAnything", "Initialize_should_not_do_anything_if_nothing_can_be_found")]
        [TestCase("MethodName_LoadFails_ReturnNull", "Method_name_returns_null_if_load_fails")]
        [TestCase("MethodName_LoadFails_ReturnsNull", "Method_name_returns_null_if_load_fails")]
        [TestCase("MethodName_SomeCondition_ArgumentExceptionThrown", "Method_name_throws_ArgumentException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_ArgumentNullExceptionThrown", "Method_name_throws_ArgumentNullException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_ArgumentOutOfRangeExceptionThrown", "Method_name_throws_ArgumentOutOfRangeException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_ExceptionThrown", "Method_name_throws_exception_if_some_condition")]
        [TestCase("MethodName_SomeCondition_FalseReturned", "Method_name_returns_false_if_some_condition")]
        [TestCase("MethodName_SomeCondition_InvalidOperationExceptionThrown", "Method_name_throws_InvalidOperationException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_NotImplementedExceptionThrown", "Method_name_throws_NotImplementedException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_NotSupportedExceptionThrown", "Method_name_throws_NotSupportedException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_NullReturned", "Method_name_returns_null_if_some_condition")]
        [TestCase("MethodName_SomeCondition_ObjectDisposedExceptionThrown", "Method_name_throws_ObjectDisposedException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_OperationCanceledExceptionThrown", "Method_name_throws_OperationCanceledException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_TaskCanceledExceptionThrown", "Method_name_throws_TaskCanceledException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_ThrowsArgumentException", "Method_name_throws_ArgumentException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_ThrowsArgumentNullException", "Method_name_throws_ArgumentNullException_if_some_condition")]
        [TestCase("MethodName_SomeCondition_ThrowsException", "Method_name_throws_exception_if_some_condition")]
        [TestCase("MethodName_SomeCondition_TrueReturned", "Method_name_returns_true_if_some_condition")]
        [TestCase("MethodName_SomeCondition_ObjectReturned", "Method_name_returns_object_if_some_condition")]
        [TestCase("MethodName_SomeCondition_Returned", "Method_name_returns_some_condition")]
        [TestCase("MethodName_WhenSomeCondition_ThrowsException", "Method_name_throws_exception_if_some_condition")]
        [TestCase("MethodName_XYZObject_ThrowsException", "Method_name_throws_exception_if_XYZ_object")]
        [TestCase("MethodName_XYZType_ThrowsException", "Method_name_throws_exception_if_XYZ_type")]
        [TestCase("MethodName_XYZReference_ThrowsException", "Method_name_throws_exception_if_XYZ_reference")]
        [TestCase("MethodName_XYZDoesSomething_ThrowsException", "Method_name_throws_exception_if_XYZ_does_something")]
        public void Code_gets_fixed_for_2_slashes_in_(string originalName, string fixedName) => VerifyCSharpFix(
                                                                                                                "class TestMe { [Test] public void " + originalName + "() { } }",
                                                                                                                "class TestMe { [Test] public void " + fixedName + "() { } }");

        [TestCase("MethodName_ShouldDoSomething_And_SomethingMore", "Method_name_should_do_something_and_something_more")]
        [TestCase("MethodName_SomeCondition_LoadFails_ReturnNull", "Method_name_returns_null_if_load_fails_and_some_condition")]
        [TestCase("MethodName_SomeCondition_LoadFails_ReturnTrue", "Method_name_returns_true_if_load_fails_and_some_condition")]
        [TestCase("MethodName_SomeCondition_LoadFails_ReturnFalse", "Method_name_returns_false_if_load_fails_and_some_condition")]
        [TestCase("MethodName_WhenSome_Stuff_ReturnsTrue", "Method_name_returns_true_if_some_stuff")]
        [TestCase("MethodName_WhenSome_Stuff_ReturnsFalse", "Method_name_returns_false_if_some_stuff")]
        [TestCase("MethodName_WhenSome_Stuff_ReturnsNull", "Method_name_returns_null_if_some_stuff")]
        [TestCase("MethodName_WhenSome_Stuff_TrueReturned", "Method_name_returns_true_if_some_stuff")]
        [TestCase("MethodName_WhenSome_Stuff_FalseReturned", "Method_name_returns_false_if_some_stuff")]
        [TestCase("MethodName_WhenSome_Stuff_NullReturned", "Method_name_returns_null_if_some_stuff")]
        [TestCase("MethodName_WhenSomething_Expect_NullReturned", "Method_name_returns_null_if_something")]
        public void Code_gets_fixed_for_3_slashes_in_(string originalName, string fixedName) => VerifyCSharpFix(
                                                                                                         "class TestMe { [Test] public void " + originalName + "() { } }",
                                                                                                         "class TestMe { [Test] public void " + fixedName + "() { } }");

        protected override string GetDiagnosticId() => MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1111_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1111_CodeFixProvider();
    }
}