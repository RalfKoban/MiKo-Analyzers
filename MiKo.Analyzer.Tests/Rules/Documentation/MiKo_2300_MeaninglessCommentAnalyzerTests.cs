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
                                                        "adds ",
                                                        "build ",
                                                        "builds ",
                                                        "calculate ",
                                                        "calculates ",
                                                        "call ",
                                                        "calls ",
                                                        "check ",
                                                        "checks ",
                                                        "close ",
                                                        "closes ",
                                                        "compare ",
                                                        "compares ",
                                                        "convert ",
                                                        "converts ",
                                                        "count ",
                                                        "counts ",
                                                        "create ",
                                                        "creates ",
                                                        "decr.",
                                                        "decrease ",
                                                        "decreases ",
                                                        "decrement ",
                                                        "decrements ",
                                                        "determine ",
                                                        "determines ",
                                                        "evaluate event arg", // no space at the end to allow combinations of the word
                                                        "get ",
                                                        "gets ",
                                                        "has ",
                                                        "if ",
                                                        "incr.",
                                                        "increase ",
                                                        "increases ",
                                                        "increment ",
                                                        "increments ",
                                                        "initialize", // no space at the end to allow combinations of the word
                                                        "invoke", // no space at the end to allow combinations of the word
                                                        "is ",
                                                        "iterate", // no space at the end to allow combinations of the word
                                                        "load",  // no space at the end to allow combinations of the word
                                                        "open ",
                                                        "opens ",
                                                        "raise ",
                                                        "raises ",
                                                        "remove ",
                                                        "removes ",
                                                        "retrieve ",
                                                        "retrieves ",
                                                        "return", // no space at the end to allow combinations of the word
                                                        "save",  // no space at the end to allow combinations of the word
                                                        "set ",
                                                        "sets ",
                                                        "start ",
                                                        "starts ",
                                                        "stop ",
                                                        "stops ",
                                                        "use", // no space at the end to allow combinations of the word
                                                    };

        private static readonly string[] AllowedComments =
                                                           {
                                                               "blank by intent",
                                                               "checked by",
                                                               "do nothing here",
                                                               "do nothing",
                                                               "ignore this",
                                                               "ignore",
                                                               "intentionally left empty",
                                                               "No-Op",
                                                               "not needed",
                                                               "nothing to do",
                                                               "special handling",
                                                               "initializers are allowed",
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
        public void No_issue_is_reported_for_website_link_in_comment_([Values("http://", "https://")] string link) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // see " + link + @"
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_commented_method_with_small_comment_([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_incorrectly_commented_method_with_long_comment_([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @" in addition to something much longer
    }
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_commented_method_with_arrow_inside_comment_([Values("", " ")] string gap) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + @" in addition to something much longer -> there is the arrow
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_incorrectly_commented_method_with_small_comment_but_escaped_Comments_([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        ////" + gap + comment + @"
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_comment_([Values("", " ")] string gap) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + @"
    }
}
");

        [Test]
        public void No_issue_is_reported_for_hex_number_in_comment_([Values("", " ")] string gap) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + @"0xDeadBeef
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_allowed_text_in_comment_([Values("", " ")] string gap, [ValueSource(nameof(AllowedComments))] string comment) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_ReSharper_formatter_advice_comment_([Values("", " ")] string gap, [Values("@formatter:off", "@formatter:on")] string comment) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
        int i = 0;
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_separator_comment_([Values("", " ")] string gap, [Values("----", "****", "====", "####")] string comment) => An_issue_is_reported_for(@"

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