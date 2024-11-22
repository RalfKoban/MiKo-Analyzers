using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    internal sealed class MiKo_1113_TestMethodsShouldNotBeNamedGivenWhenThenAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AcceptedMethodNames =
                                                               [
                                                                   "GivenSomething",
                                                                   "Given_something",
                                                                   "GivenSomething_ThenNothing",
                                                                   "Given_something_then_nothing",
                                                                   "GivenSomething_WhenAnything",
                                                                   "Given_something_when_anything",
                                                               ];

        private static readonly string[] WrongMethodNames =
                                                            [
                                                                "GivenSomething_WhenAnything_ThenNothing",
                                                                "Given_something_when_there_is_anything_then_nothing_happens",
                                                            ];

        [Test]
        public void No_issue_is_reported_for_test_method_with_correct_name_(
                                                                        [ValueSource(nameof(AcceptedMethodNames))] string methodName,
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

        protected override string GetDiagnosticId() => MiKo_1113_TestMethodsShouldNotBeNamedGivenWhenThenAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1113_TestMethodsShouldNotBeNamedGivenWhenThenAnalyzer();
    }
}
