using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1119_TestMethodsWhenPresentAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ProblematicTexts =
                                                            [
                                                                "if_present",
                                                                "if_not_present",
                                                                "when_present",
                                                                "when_not_present",
                                                                "IfPresent",
                                                                "IfNotPresent",
                                                                "If_Present",
                                                                "If_Not_Present",
                                                                "If_NotPresent",
                                                                "WhenPresent",
                                                                "WhenNotPresent",
                                                                "When_Present",
                                                                "When_Not_Present",
                                                                "When_NotPresent",
                                                            ];

        private static readonly string[] UnproblematicNames =
                                                              [
                                                                  "DoSomething",
                                                                  "DoSomethingIfPresentationIsShown",
                                                                  "DoSomethingWhenPresentationIsShown",
                                                                  "DoSomethingIfPresentingSomething",
                                                                  "DoSomethingWhenPresentingSomething",
                                                                  "DoSomethingIfNotPresentingSomething",
                                                                  "DoSomethingWhenNotPresentingSomething",
                                                                  "DoSomething_If_Presentation_Is_Shown",
                                                                  "DoSomething_When_Presentation_Is_Shown",
                                                                  "DoSomething_If_Presenting_Something",
                                                                  "DoSomething_When_Presenting_Something",
                                                                  "DoSomething_If_Not_Presenting_Something",
                                                                  "DoSomething_When_Not_Presenting_Something",
                                                                  "DoSomething_If_NotPresenting_Something",
                                                                  "DoSomething_When_NotPresenting_Something",
                                                                  "DoSomething_if_presentation_is_shown",
                                                                  "DoSomething_when_presentation_is_shown",
                                                                  "DoSomething_if_presenting_something",
                                                                  "DoSomething_when_presenting_something",
                                                                  "DoSomething_if_not_presenting_something",
                                                                  "DoSomething_when_not_presenting_something",
                                                              ];

        [Test]
        public void No_issue_is_reported_for_test_method_with_correct_name_(
                                                                        [ValueSource(nameof(TestFixtures))] string fixture,
                                                                        [ValueSource(nameof(Tests))] string test,
                                                                        [ValueSource(nameof(UnproblematicNames))] string unproblematicTest)
            => No_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void " + unproblematicTest + @"() { }
}
");

        [Test]
        public void An_issue_is_reported_for_test_method_ending_with_incorrect_name_(
                                                                                 [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                 [ValueSource(nameof(Tests))] string test,
                                                                                 [ValueSource(nameof(ProblematicTexts))] string problematicTest)
            => An_issue_is_reported_for(@"
[" + fixture + @"]
public class TestMe
{
    [" + test + @"]
    public void DoSomething" + problematicTest + @"() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1119_TestMethodsWhenPresentAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1119_TestMethodsWhenPresentAnalyzer();
    }
}