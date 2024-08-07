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
    public void DoSomething()
    {
        // some comment
    }
}");

        [Test]
        public void No_issue_is_reported_for_intentionally_comment_with_reason_([ValueSource(nameof(IntentionalPhrases))] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // " + comment + @", reason: we like it this way
    }
}");

        [Test]
        public void No_issue_is_reported_for_intentionally_comment_with_because_([ValueSource(nameof(IntentionalPhrases))] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // " + comment + @" because we like it this way
    }
}");

        [Test]
        public void No_issue_is_reported_for_intentionally_comment_with_reason_in_catch_block_([ValueSource(nameof(IntentionalPhrases))] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        try
        {
            DoSomething();
        }
        catch
        {
            // " + comment + @", reason: we like it this way
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_intentionally_comment_with_because_in_catch_block_([ValueSource(nameof(IntentionalPhrases))] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        try
        {
            DoSomething();
        }
        catch
        {
            // " + comment + @" because we like it this way
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_intentionally_comment_with_reason_on_field_([ValueSource(nameof(IntentionalPhrases))] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    public int MyField; // " + comment + @", reason: we like it this way
}");

        [Test]
        public void No_issue_is_reported_for_intentionally_comment_with_because_on_field_([ValueSource(nameof(IntentionalPhrases))] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    public int MyField; // " + comment + @" because we like it this way
}");

        [Test]
        public void An_issue_is_reported_for_wrong_comment_([ValueSource(nameof(IntentionalPhrases))] string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // " + comment + @"
    }
}");

        [Test]
        public void An_issue_is_reported_for_wrong_comment_in_catch_block_([ValueSource(nameof(IntentionalPhrases))] string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        try
        {
            DoSomething();
        }
        catch
        {
            // " + comment + @"
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_wrong_comment_on_field_([ValueSource(nameof(IntentionalPhrases))] string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    public int MyField; // " + comment + @"
}");

        protected override string GetDiagnosticId() => MiKo_2310_CommentContainsIntentionallyAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2310_CommentContainsIntentionallyAnalyzer();
    }
}