using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectMethodNames =
                                                              [
                                                                  "DoSomething_does_something",
                                                                  "Returns_null_if_load_fails",
                                                                  "Throws_ArgumentNullException_under_some_conditions",
                                                                  "Method_name_returns_false_if_load_fails_and_some_condition",
                                                              ];

        private static readonly string[] WrongMethodNames =
                                                            [
                                                                "MethodName_Scenario_ExpectedOutcome",
                                                                "DoSomething_WithUnnecessaryData_ReturnsTrue",
                                                                "DoSomething_WithUnnecessaryDataForSomething_ThrowsException",
                                                            ];

        [Test]
        public void No_issue_is_reported_for_non_test_class() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething() { }

    public void DoSomething(int i) { }
}
");

        [Test]
        public void No_issue_is_reported_for_test_method_with_correct_name_(
                                                                        [ValueSource(nameof(CorrectMethodNames))] string methodName,
                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                        [ValueSource(nameof(Tests))] string test)
        => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void " + methodName + @"() { }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_with_wrong_name_(
                                                                      [ValueSource(nameof(WrongMethodNames))] string methodName,
                                                                      [ValueSource(nameof(TestFixtures))] string fixture,
                                                                      [ValueSource(nameof(Tests))] string test)
            => An_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void " + methodName + @"() { }
}
");

        [TestCase("DoesSomething_IsExceptional", "Throws_exception_if_it_does_something")]
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
        [TestCase("ContainsData_TriesToDoStuff", "Tries_to_do_stuff_if_it_contains_data")]
        [TestCase("Method_ThrowsExceptionIfNull", "Method_throws_exception_if_null")]
        [TestCase("Method_ThrowsArgumentNullExceptionIfNull", "Method_throws_ArgumentNullException_if_null")]
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
        [TestCase("MethodName_SomeCondition_NoNullReferenceExceptionThrown", "Method_name_throws_no_NullReferenceException_if_some_condition")]
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
        [TestCase("MethodName_SomeCondition_GuidEmptyReturned", "Method_name_returns_empty_guid_if_some_condition")]
        [TestCase("MethodName_SomeCondition_StringEmptyReturned", "Method_name_returns_empty_string_if_some_condition")]
        [TestCase("MethodName_SomeCondition_Returned", "Method_name_returns_some_condition")]
        [TestCase("MethodName_SomeCondition_TryToDoStuff", "Method_name_try_to_do_stuff_if_some_condition")]
        [TestCase("MethodName_SomeCondition_TriesToDoStuff", "Method_name_tries_to_do_stuff_if_some_condition")]
        [TestCase("MethodName_SomeCondition_CallStuff", "Method_name_call_stuff_if_some_condition")]
        [TestCase("MethodName_SomeCondition_CallsStuff", "Method_name_calls_stuff_if_some_condition")]
        [TestCase("MethodName_SomeCondition_InvokeStuff", "Method_name_invoke_stuff_if_some_condition")]
        [TestCase("MethodName_SomeCondition_InvokesStuff", "Method_name_invokes_stuff_if_some_condition")]
        [TestCase("MethodName_WhenSomeCondition_ThrowsException", "Method_name_throws_exception_if_some_condition")]
        [TestCase("MethodName_XYZObject_ThrowsException", "Method_name_throws_exception_if_XYZ_object")]
        [TestCase("MethodName_XYZType_ThrowsException", "Method_name_throws_exception_if_XYZ_type")]
        [TestCase("MethodName_XYZReference_ThrowsException", "Method_name_throws_exception_if_XYZ_reference")]
        [TestCase("MethodName_XYZDoesSomething_ThrowsException", "Method_name_throws_exception_if_XYZ_does_something")]
        [TestCase("MethodName_SomeConditionIsMet_ThrowsException", "Method_name_throws_exception_if_some_condition_is_met")]
        [TestCase("MethodName_SomeConditionIsMet_RethrowsException", "Method_name_rethrows_exception_if_some_condition_is_met")]
        [TestCase("MethodName_ThrowsException_IfSomeConditionIsMet", "Method_name_throws_exception_if_some_condition_is_met")]
        [TestCase("MethodName_ThrowsArgumentException_IfSomeConditionIsMet", "Method_name_throws_ArgumentException_if_some_condition_is_met")]
        [TestCase("MethodName_RetrievesSomething_TriesToDoStuff", "Method_name_tries_to_do_stuff_if_it_retrieves_something")]
        [TestCase("MethodName_ReceivesSomething_TriesToDoStuff", "Method_name_tries_to_do_stuff_if_it_receives_something")]
        [TestCase("MethodName_GetsSomething_TriesToDoStuff", "Method_name_tries_to_do_stuff_if_it_gets_something")]
        [TestCase("MethodName_ContainsSomething_TriesToDoStuff", "Method_name_tries_to_do_stuff_if_it_contains_something")]
        [TestCase("MethodName_GivenSomething_NoError", "Method_name_has_no_error_if_something_is_given")]
        [TestCase("MethodName_GivenSomething_HasNoError", "Method_name_has_no_error_if_something_is_given")]
        [TestCase("MethodName_GivenSomething_HasError", "Method_name_has_error_if_something_is_given")]
        [TestCase("MethodName_EverythingFine_SendsData", "Method_name_sends_data_if_everything_is_fine")]
        [TestCase("MethodName_NoLongerThrow_NullReferenceException", "Method_name_throws_no_NullReferenceException")]
        [TestCase("MethodName_NoLongerThrows_NullReferenceException", "Method_name_throws_no_NullReferenceException")]
        [TestCase("MethodName_NotThrow_NullReferenceException", "Method_name_throws_no_NullReferenceException")]
        [TestCase("MethodName_NotThrows_NullReferenceException", "Method_name_throws_no_NullReferenceException")]
        [TestCase("MethodName_ConsumedMessage_Publish", "Method_name_publish_consumed_message")]
        [TestCase("MethodName_ConsumedMessage_Publishes", "Method_name_publishes_consumed_message")]
        [TestCase("MethodName_RejectedConsumedMessage_LogsWhenAboveCriticalThreshold", "Method_name_logs_if_above_critical_threshold_if_consumed_message_is_rejected")]
        [TestCase("MethodName_RejectedConsumedMessage_RejectsMessage", "Method_name_rejects_message_if_consumed_message_is_rejected")]
        public void Code_gets_fixed_for_2_slashes_in_(string originalName, string fixedName) => VerifyCSharpFix(
                                                                                                            "class TestMe { [Test] public void " + originalName + "() { } }",
                                                                                                            "class TestMe { [Test] public void " + fixedName + "() { } }");

        [TestCase("MethodName_ShouldDoSomething_And_SomethingMore", "Method_name_should_do_something_and_something_more")]
        [TestCase("MethodName_SomeCondition_LoadFails_ReturnNull", "Method_name_returns_null_if_load_fails_and_some_condition")]
        [TestCase("MethodName_SomeCondition_LoadFails_ReturnTrue", "Method_name_returns_true_if_load_fails_and_some_condition")]
        [TestCase("MethodName_SomeCondition_LoadFails_ReturnFalse", "Method_name_returns_false_if_load_fails_and_some_condition")]
        [TestCase("MethodName_SomeCondition_ContainsData_TriesToDoStuff", "Method_name_tries_to_do_stuff_if_it_contains_data_and_some_condition")]
        [TestCase("MethodName_WhenSome_Stuff_ReturnsTrue", "Method_name_returns_true_if_some_stuff")]
        [TestCase("MethodName_WhenSome_Stuff_ReturnsFalse", "Method_name_returns_false_if_some_stuff")]
        [TestCase("MethodName_WhenSome_Stuff_ReturnsNull", "Method_name_returns_null_if_some_stuff")]
        [TestCase("MethodName_WhenSome_Stuff_TrueReturned", "Method_name_returns_true_if_some_stuff")]
        [TestCase("MethodName_WhenSome_Stuff_FalseReturned", "Method_name_returns_false_if_some_stuff")]
        [TestCase("MethodName_WhenSome_Stuff_NullReturned", "Method_name_returns_null_if_some_stuff")]
        [TestCase("MethodName_WhenSomething_Expect_NullReturned", "Method_name_returns_null_if_something")]
        [TestCase("MethodName_ThrowsException_if_SomeConditionIsMet", "Method_name_throws_exception_if_some_condition_is_met")]
        [TestCase("MethodName_ThrowsArgumentException_if_SomeConditionIsMet", "Method_name_throws_ArgumentException_if_some_condition_is_met")]
        [TestCase("MethodName_NoLongerThrowsNullReferenceException_if_SomeConditionIsMet", "Method_name_throws_no_NullReferenceException_if_some_condition_is_met")]
        [TestCase("MethodName_NotThrowsNullReferenceException_if_SomeConditionIsMet", "Method_name_throws_no_NullReferenceException_if_some_condition_is_met")]
        public void Code_gets_fixed_for_3_slashes_in_(string originalName, string fixedName) => VerifyCSharpFix(
                                                                                                            "class TestMe { [Test] public void " + originalName + "() { } }",
                                                                                                            "class TestMe { [Test] public void " + fixedName + "() { } }");

        protected override string GetDiagnosticId() => MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1115_TestMethodsShouldNotBeNamedScenarioExpectedOutcomeAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_1115_CodeFixProvider();
    }
}