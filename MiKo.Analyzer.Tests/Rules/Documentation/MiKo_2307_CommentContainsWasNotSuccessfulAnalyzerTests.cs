using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2307_CommentContainsWasNotSuccessfulAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_correct_comment() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // some comment
    }
}");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // This was not successful
    }
}");

        protected override string GetDiagnosticId() => MiKo_2307_CommentContainsWasNotSuccessfulAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2307_CommentContainsWasNotSuccessfulAnalyzer();
    }
}