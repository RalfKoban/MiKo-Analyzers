using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2226_DocumentationContainsIntentionallyAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] IntentionalPhrases =
                                                              [
                                                                  "left empty by intent",
                                                                  "left empty by intention",
                                                                  "left empty intentionally",
                                                                  "left empty intentionaly", // check for typo
                                                                  "intentionally empty",
                                                                  "intentionaly empty", // check for typo
                                                                  "empty with intent",
                                                                  "empty with intention",
                                                                  "empty on purpose",
                                                                  "left empty on purpose",
                                                                  "on purpose left empty",
                                                                  "purposely left empty",
                                                                  "purposly left empty", // check for typo
                                                                  "by indent", // check for typo
                                                                  "empty with indent", // check for typo
                                                                  "empty with indention", // check for typo
                                                                  "indentionally", // check for typo
                                                                  "indentionaly", // check for typo
                                                                  "does not matter",
                                                                  "doesn't matter",
                                                                  "doesnt matter", // check for typo
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
        public void No_issue_is_reported_for_intentionally_documentation_with_reason_([ValueSource(nameof(IntentionalPhrases))] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + comment + @", reason is we like it.
    /// </summary>
    public void DoSomething()
    {
    }
}");

        [Test]
        public void No_issue_is_reported_for_intentionally_documentation_with_because_([ValueSource(nameof(IntentionalPhrases))] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// " + comment + @" because we like it.
    /// </summary>
    public void DoSomething()
    {
    }
}");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation_([ValueSource(nameof(IntentionalPhrases))] string comment) => An_issue_is_reported_for(@"
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