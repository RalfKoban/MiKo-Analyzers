using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2226_DocumentationContainsIntentionallyAnalyzerTests : CodeFixVerifier
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
    /// <summary>
    /// some comment
    /// </summary>
    public void DoSomething()
    {
    }
}");

        [TestCase("left empty by intent")]
        [TestCase("left empty by intention")]
        [TestCase("left empty intentionally")]
        [TestCase("intentionally empty")]
        [TestCase("empty with intent")]
        [TestCase("empty with intention")]
        [TestCase("left empty on purpose")]
        [TestCase("on purpose left empty")]
        [TestCase("purposely left empty")]
        [TestCase("purposly left empty")] // check for typo
        public void An_issue_is_reported_for_wrong_documentation_(string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    public void DoSomething()
    {
    }
}");

        protected override string GetDiagnosticId() => MiKo_2226_DocumentationContainsIntentionallyAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2226_DocumentationContainsIntentionallyAnalyzer();
    }
}