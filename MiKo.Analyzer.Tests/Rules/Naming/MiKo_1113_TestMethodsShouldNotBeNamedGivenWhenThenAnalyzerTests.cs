using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    internal sealed class MiKo_1113_TestMethodsShouldNotBeNamedGivenWhenThenAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AcceptedMethodNames =
            {
                "GivenSomething",
                "Given_something",
                "GivenSomething_ThenNothing",
                "Given_something_then_nothing",
                "GivenSomething_WhenAnything",
                "Given_something_when_anything",
            };

        private static readonly string[] WrongMethodNames =
            {
                "GivenSomething_WhenAnything_ThenNothing",
                "Given_something_when_there_is_anything_then_nothing_happens",
            };

        [Test, Combinatorial]
        public void No_issue_is_reported_for_test_method_with_correct_name_(
                                                                [ValueSource(nameof(TestFixtures))] string testFixture,
                                                                [ValueSource(nameof(Tests))] string test,
                                                                [ValueSource(nameof(AcceptedMethodNames))] string methodName)
            => No_issue_is_reported_for(@"

[" + testFixture + @"]
public class TestMe
{
    [" + test + @"]
    public void " + methodName + @"() { }
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

        protected override string GetDiagnosticId() => MiKo_1113_TestMethodsShouldNotBeNamedGivenWhenThenAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1113_TestMethodsShouldNotBeNamedGivenWhenThenAnalyzer();
    }
}
