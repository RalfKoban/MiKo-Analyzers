using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    internal sealed class MiKo_1114_TestMethodsShouldNotBeNamedBadOrHappyPathAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AcceptedMethodNames =
                                                               [
                                                                   "DoSomething",
                                                               ];

        private static readonly string[] WrongMethodNames =
                                                            [
                                                                "Bad_case",
                                                                "Bad_Case",
                                                                "bad_path",
                                                                "Bad_Path",
                                                                "BadCase",
                                                                "BadPath",
                                                                "Good_case",
                                                                "Good_Case",
                                                                "Good_path",
                                                                "Good_Path",
                                                                "GoodCase",
                                                                "GoodPath",
                                                                "happy_case",
                                                                "Happy_Case",
                                                                "happy_path",
                                                                "Happy_Path",
                                                                "HappyCase",
                                                                "HappyPath",
                                                            ];

        [Test]
        public void No_issue_is_reported_for_test_method_with_correct_name_([ValueSource(nameof(AcceptedMethodNames))] string methodName)
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
");
                                            }
                                        }
                                    });

        protected override string GetDiagnosticId() => MiKo_1114_TestMethodsShouldNotBeNamedBadOrHappyPathAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1114_TestMethodsShouldNotBeNamedBadOrHappyPathAnalyzer();
    }
}