using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2302_CommentedOutCodeAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Comments =
            {
                "var x = 42;",
                "string s = x.ToString();",
                "if (i == 42) ",
                "switch (expression)",
                "case 0815:",
                "DoSomething();",
                "public abstract void DoSomething();",
                "internal abstract void DoSomething();",
                "protected abstract void DoSomething();",
                "private int _field;",
                "return null ?? 42;",
                "i++;",
                "e.Handled = true;",
                "else",
                "return true || false;",
                "return true && false;",
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

        [Test, Combinatorial]
        public void An_issue_is_reported_for_commented_out_code_in_method_([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_multiline_commented_out_code_in_method_([Values("", " ")] string gap, [ValueSource(nameof(Comments))] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //
        //" + gap + comment + @"
        //
    }
}
");

        [Test]
        public void No_issue_is_reported_for_normal_comment_with_else_([Values("", " ")] string gap) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + @"else or anything
    }
}
");

        [Test]
        public void No_issue_is_reported_for_website_link_([Values("", " ")] string gap) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + @"See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
    }
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_ReSharper_comment_([Values("", " ", "//", "// ")] string gap, [Values("ReSharper disable whatever", "ReSharper restore whatever")] string resharperText) => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + resharperText + @"
    }
}
");

        [Test]
        public void No_issue_is_reported_for_framed_comment() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        /////////////////////
        //                 //
        // Framed comment. //
        //                 //
        /////////////////////
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_string_interpolation_comment() => An_issue_is_reported_for(@"
using System;
using System.Linq;

public class TestMe
{
    public void DoSomething()
    {
        // DoSomething($""42"")
        //    .FirstOrDefault();
    }

    public string DoSomething(int index) => string.Empty;
}
", 2);

        protected override string GetDiagnosticId() => MiKo_2302_CommentedOutCodeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2302_CommentedOutCodeAnalyzer();
    }
}