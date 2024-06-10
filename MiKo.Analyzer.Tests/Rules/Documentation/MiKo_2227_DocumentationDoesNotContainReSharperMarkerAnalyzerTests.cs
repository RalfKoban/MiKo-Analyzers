using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2227_DocumentationDoesNotContainReSharperMarkerAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ReSharperMarkers =
                                                            [
                                                                "ReSharper disable",
                                                                "ReSharper disable once",
                                                                "ReSharper restore",
                                                                "ReSharper disable Whatever",
                                                                "ReSharper disable once Whatever",
                                                                "ReSharper restore Whatever",
                                                            ];

        [Test]
        public void No_issue_is_reported_for_undocumented_class() => No_issue_is_reported_for(@"
public class TestMe
{
}");

        [Test]
        public void No_issue_is_reported_for_correct_comment() => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// some comment
    /// </summary>
    public void DoSomething()
    {
    }
}");

        [Test]
        public void No_issue_is_reported_for_ReSharper_suppression_in_source_code_([ValueSource(nameof(ReSharperMarkers))] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// some comment
    /// </summary>
    public void DoSomething()
    {
        // " + comment + @"
    }
}");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation_([ValueSource(nameof(ReSharperMarkers))] string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    public void DoSomething()
    {
    }
}");

        protected override string GetDiagnosticId() => MiKo_2227_DocumentationDoesNotContainReSharperMarkerAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2227_DocumentationDoesNotContainReSharperMarkerAnalyzer();
    }
}