using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2310_CommentContainsIntentionallyAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] IntentionalPhrases =
                                                              [
                                                                  "left empty by intent",
                                                                  "left empty by intention",
                                                                  "left empty intentionally",
                                                                  "intentionally empty",
                                                                  "empty with intent",
                                                                  "empty with intention",
                                                                  "left empty on purpose",
                                                                  "on purpose left empty",
                                                                  "purposely left empty",
                                                                  "purposly left empty", // check for typo
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
    public void DoSomething()
    {
        // some comment
    }
}");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation_([ValueSource(nameof(IntentionalPhrases))] string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // " + comment + @"
    }
}");

        [Test]
        public void An_issue_is_reported_for_wrong_documentation_on_field_([ValueSource(nameof(IntentionalPhrases))] string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    public int MyField; // " + comment + @"
}");

        protected override string GetDiagnosticId() => MiKo_2310_CommentContainsIntentionallyAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2310_CommentContainsIntentionallyAnalyzer();
    }
}