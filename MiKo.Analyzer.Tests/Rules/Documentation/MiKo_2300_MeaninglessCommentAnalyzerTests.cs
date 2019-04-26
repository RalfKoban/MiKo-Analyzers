using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2300_MeaninglessCommentAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Comments =
            {
                "add ",
                "calculate ",
                "call ",
                "check ",
                "close ",
                "compare ",
                "convert ",
                "count ",
                "create ",
                "decr.",
                "decrease ",
                "decrement ",
                "determine ",
                "determines ",
                "evaluate event arg",
                "get ",
                "has ",
                "if ",
                "incr.",
                "increase ",
                "increment ",
                "initialize ",
                "invoke" ,
                "is " ,
                "open ",
                "raise ",
                "remove ",
                "retrieve ",
                "return ",
                "set ",
                "start ",
                "stop ",
            };

        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_method() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment that is long enough
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_commented_method_with_line_break_where_second_comment_is_short_enough_to_trigger_an_issue() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment that is long
        // enough
    }
}
");

        [Test]
        public void No_issue_is_reported_for_website_link_in_comment([Values("http://","https://")] string link) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // see " + link + @"
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_commented_method_with_small_comment([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_commented_method_with_long_comment([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @" in addition to something much longer
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_method_with_arrow_inside_comment([Values("", " ")] string gap) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + @" in addition to something much longer -> there is the arrow
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_incorrectly_commented_method_with_small_comment_but_escaped_Comments([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        ////" + gap + comment + @"
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_comment([Values("", " ")] string gap) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + @"
    }
}
");

        [Test]
        public void No_issue_is_reported_for_hex_number_in_comment([Values("", " ")] string gap) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + @"0xDeadBeef
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ignore_text_in_comment([Values("", " ")] string gap, [Values("ignore", "ignore this")] string comment) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nothing_to_do_text_in_comment([Values("", " ")] string gap, [Values("nothing to do", "do nothing", "do nothing here")] string comment) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2300_MeaninglessCommentAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2300_MeaninglessCommentAnalyzer();
    }
}