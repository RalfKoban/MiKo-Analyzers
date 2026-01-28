using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1523_HelperMethodAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongTerms = ["Helper", "HelpingMethod"];

        [Test]
        public void No_issue_is_reported_for_method_with_correct_name() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void DoSomething(int something)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_starting_with_([ValueSource(nameof(WrongTerms))] string term) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void " + term + @"Something(int something)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_ending_with_([ValueSource(nameof(WrongTerms))] string term) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private void Something" + term + @"(int something)
        {
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_1523_HelperMethodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1523_HelperMethodAnalyzer();
    }
}