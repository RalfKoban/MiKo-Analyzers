using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_correct_comment() => No_issue_is_reported_for(@"
/// <summary>
/// Some summary.
/// </summary>
public class TestMe
{
}");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation() => An_issue_is_reported_for(@"
/// <summary>
/// This was not successful.
/// </summary>
public class TestMe
{
}");

        protected override string GetDiagnosticId() => MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer();
    }
}