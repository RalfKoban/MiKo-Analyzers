using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1117_TestMethodsShouldBeNamedMorePreciseAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectMethodNames =
                                                              [
                                                                  "Something_NoExceptionThrown",
                                                                  "Something_raises_Whatever_event",
                                                                  "Something_" + nameof(ArgumentException) + "Thrown",
                                                                  "Something_" + nameof(ArgumentNullException) + "Thrown",
                                                                  "Something_" + nameof(ArgumentOutOfRangeException) + "Thrown",
                                                                  "Something_" + nameof(InvalidOperationException) + "Thrown",
                                                                  "Something_" + nameof(KeyNotFoundException) + "Thrown",
                                                                  "Something_" + nameof(NotImplementedException) + "Thrown",
                                                                  "Something_" + nameof(NotSupportedException) + "Thrown",
                                                                  "Something_" + nameof(NullReferenceException) + "Thrown",
                                                                  "Something_" + nameof(ObjectDisposedException) + "Thrown",
                                                                  "Something_" + nameof(OperationCanceledException) + "Thrown",
                                                                  "Something_" + nameof(TaskCanceledException) + "Thrown",
                                                                  "Something_" + nameof(UnauthorizedAccessException) + "Thrown",
                                                                  "Something_ValidationExceptionThrown",
                                                                  "Something_JsonExceptionThrown",
                                                                  "Something_throws_" + nameof(ArgumentException),
                                                                  "Something_throws_" + nameof(ArgumentNullException),
                                                                  "Something_throws_" + nameof(ArgumentOutOfRangeException),
                                                                  "Something_throws_" + nameof(InvalidOperationException),
                                                                  "Something_throws_" + nameof(KeyNotFoundException),
                                                                  "Something_throws_" + nameof(NotImplementedException),
                                                                  "Something_throws_" + nameof(NotSupportedException),
                                                                  "Something_throws_" + nameof(NullReferenceException),
                                                                  "Something_throws_" + nameof(ObjectDisposedException),
                                                                  "Something_throws_" + nameof(OperationCanceledException),
                                                                  "Something_throws_" + nameof(TaskCanceledException),
                                                                  "Something_throws_" + nameof(UnauthorizedAccessException),
                                                                  "Something_throws_JsonException",
                                                                  "Something_throws_ValidationException",
                                                                  "Something_with_a_property",
                                                                  "Something_with_an_uppercase_Property",
                                                              ];

        private static readonly string[] VagueMethodNames =
                                                            [
                                                                "Something_EventRaised",
                                                                "Something_EventIsRaised",
                                                                "Something_IfEventIsRaised",
                                                                "Something_WhenEventIsRaised",
                                                                "Something_Event_Is_Raised",
                                                                "Something_If_Event_Is_Raised",
                                                                "Something_When_Event_Is_Raised",
                                                                "Something_RaisedEvent",
                                                                "Something_RaisesEvent",
                                                                "Something_Raised_Event",
                                                                "Something_Raises_Event",
                                                                "Something_EventOccurred",
                                                                "Something_EventOccured",
                                                                "Something_OccurredEvent",
                                                                "Something_OccuredEvent",
                                                                "Something_EventIsFired",
                                                                "Something_EventFired",
                                                                "Something_FiresEvent",
                                                                "Something_ExceptionThrown",
                                                                "Something_ThrowsException",
                                                                "Something_exception_thrown",
                                                                "Something_throws_exception",
                                                                "Something_correct",
                                                                "Something_correctly",
                                                                "Something_do_not_handle",
                                                                "Something_does_not_handle",
                                                                "Something_DoesNotHandle",
                                                                "Something_done",
                                                                "Something_DoNotHandle",
                                                                "Something_finished",
                                                                "Something_gracefully",
                                                                "Something_handles",
                                                                "Something_improperly",
                                                                "Something_incorrect",
                                                                "Something_incorrectly",
                                                                "Something_is_acceptable",
                                                                "Something_is_inacceptable",
                                                                "Something_is_unacceptable",
                                                                "Something_normally",
                                                                "Something_proper_way",
                                                                "Something_properly",
                                                                "Something_properWay",
                                                                "Something_successfully",
                                                                "Something_works",
                                                                "SomethingDoesNotHandle",
                                                                "SomethingDoNotHandle",
                                                                "SomethingHandles",
                                                                "SomethingIsAcceptable",
                                                                "SomethingIsCorrect",
                                                                "SomethingIsCorrectly",
                                                                "SomethingIsDone",
                                                                "SomethingIsFinished",
                                                                "SomethingIsGraceful",
                                                                "SomethingIsGracefully",
                                                                "SomethingIsImproper",
                                                                "SomethingIsImproperly",
                                                                "SomethingIsInacceptable",
                                                                "SomethingIsIncorrect",
                                                                "SomethingIsIncorrectly",
                                                                "SomethingIsNormal",
                                                                "SomethingIsNormally",
                                                                "SomethingIsProperly",
                                                                "SomethingIsProperWay",
                                                                "SomethingIsSuccessful",
                                                                "SomethingIsSuccessfully",
                                                                "SomethingIsUnacceptable",
                                                                "SomethingWorks",
                                                            ];

        [Test]
        public void No_issue_is_reported_for_non_test_class_([ValueSource(nameof(VagueMethodNames))] string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
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
                                                                      [ValueSource(nameof(VagueMethodNames))] string methodName,
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

        protected override string GetDiagnosticId() => MiKo_1117_TestMethodsShouldBeNamedMorePreciseAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1117_TestMethodsShouldBeNamedMorePreciseAnalyzer();
    }
}