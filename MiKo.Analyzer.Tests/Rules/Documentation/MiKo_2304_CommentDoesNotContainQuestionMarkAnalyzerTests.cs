using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2304_CommentDoesNotContainQuestionMarkAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_without_question_mark() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        // some comment
    }
}
");

        [Test]
        public void No_issue_is_reported_for_TODO_comment_with_question_mark_([Values("TODO:", "ToDo:", "TO DO:", "To Do:", "TODO", "ToDo", "TO DO", "To Do")] string comment) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // " + comment + @" Something missing?
    }
}
");

        [Test]
        public void An_issue_is_reported_for_comment_with_question_mark_([Values("Wtf?", "??? That's strange", "Wtf? Does not make sense!")] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // " + comment + @"
    }
}
");

        [Test]
        public void An_issue_is_reported_for_multi_line_comment_with_question_mark_([Values("Wtf?", "??? That's strange", "Wtf? Does not make sense!")] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // " + comment + @"
        // whatever it takes
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_question_mark_in_HTML_link() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // http://machine:8080/tfs/Xyz/Abc/_workitems?id=12345
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_question_mark_and_additional_text_in_HTML_link() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // (see ID 12345 - http://machine:8080/tfs/Xyz/Abc/_workitems?id=12345)
    }
}
");

        [Test]
        public void An_issue_is_reported_for_comment_with_question_mark_in_HTML_link_but_additional_question_marks() => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // if not http://machine:8080/tfs/Xyz/Abc/_workitems?id=12345, then what shall we do?
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2304_CommentDoesNotContainQuestionMarkAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2304_CommentDoesNotContainQuestionMarkAnalyzer();
    }
}