using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2300_MeaninglessCommentAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Comments =
                                                    [
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
                                                    ];

        private static readonly string[] AllowedComments =
                                                           [
                                                               "blank by intent",
                                                               "checked by",
                                                               "do nothing here",
                                                               "do nothing",
                                                               "ignore this",
                                                               "ignore",
                                                               "initializers are allowed",
                                                               "intentionally left empty",
                                                               "it is a mock",
                                                               "it is just a mock",
                                                               "it is only a mock",
                                                               "it's a mock",
                                                               "just a mock",
                                                               "just mocked",
                                                               "No-Op",
                                                               "not needed",
                                                               "nothing to do here",
                                                               "nothing to do",
                                                               "special handling",
                                                               "ncrunch: some text here",
                                                           ];

        [Test]
        public void No_issue_is_reported_for_uncommented_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_separator_comment_([Values("", " ")] string gap, [Values("----", "****", "====", "####")] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        [Test]
        public void No_issue_is_reported_for_meaningful_comment() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // some comment that is long enough
    }
}
");

        [Test]
        public void No_issue_is_reported_for_meaningful_multi_line_comment() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_comment_with_website_link_([Values("http://", "https://")] string link) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // see " + link + @"
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_escaped_meaningless_comment_([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_comment_with_hex_number_([Values("", " ")] string gap) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        //" + gap + @"0xDeadBeef
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_comment_with_special_text_([Values("", " ")] string gap, [ValueSource(nameof(AllowedComments))] string comment) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_comment_with_ReSharper_formatter_directive_([Values("", " ")] string gap, [Values("@formatter:off", "@formatter:on")] string comment) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_comment_with_([Values("", " ")] string gap, [Values("ReSharper disable Something", "ReSharper restore Something")] string comment) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_comment_with_reason_([ValueSource(nameof(Comments))] string comment, [Values("because", "reason")] string reason) => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        // " + comment + " in addition to something much longer " + reason + @" I said it
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_too_short_comment_([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_comment_without_context_([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @" in addition to something much longer
    }
}
");

        [Test]
        public void An_issue_is_reported_for_comment_with_arrow_([Values("", " ")] string gap) => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        //" + gap + @" in addition to something much longer -> there is the arrow
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_too_short_comment_in_documented_method_([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some summary.
    /// </summary>
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_comment_without_context_in_documented_method_([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some summary.
    /// </summary>
    public void DoSomething()
    {
        //" + gap + comment + @" in addition to something much longer
    }
}
");

        [Test]
        public void An_issue_is_reported_for_comment_with_arrow_in_documented_method_([Values("", " ")] string gap) => An_issue_is_reported_for(@"
public class TestMe
{
    /// <summary>
    /// Some summary.
    /// </summary>
    public void DoSomething()
    {
        //" + gap + @" in addition to something much longer -> there is the arrow
    }
}
");

        [Test]
        public void No_issue_is_reported_for_uncommented_field_within_region() => No_issue_is_reported_for(@"
public class TestMe
{
    #region Some region

    private int _someField;

    #endregion
}
");

        protected override string GetDiagnosticId() => MiKo_2300_MeaninglessCommentAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2300_MeaninglessCommentAnalyzer();
    }
}