using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2311_CommentIsSeparatorAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Separators = ["----", "****", "====", "####"];

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
        public void An_issue_is_reported_for_separator_comment_([Values("", " ")] string gap, [ValueSource(nameof(Separators))] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    //" + gap + comment + @"

    public void DoSomething()
    {
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_separator_comment_in_method_([Values("", " ")] string gap, [ValueSource(nameof(Separators))] string comment) => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //" + gap + comment + @"
    }
}
");

        [Test, Combinatorial]
        public void Code_gets_fixed_for_separator_comment_at_begin_([Values("", " ")] string gap, [ValueSource(nameof(Separators))] string comment)
        {
            var originalCode = @"
public class TestMe
{
    //" + gap + comment + @"

    public void DoSomething()
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test, Combinatorial]
        public void Code_gets_fixed_for_separator_comment_in_between_([Values("", " ")] string gap, [ValueSource(nameof(Separators))] string comment)
        {
            var originalCode = @"
public class TestMe
{
    public void DoSomething()
    {
    }

    //" + gap + comment + @"

    public void DoSomethingElse()
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
    }

    public void DoSomethingElse()
    {
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test, Combinatorial]
        public void Code_gets_fixed_for_separator_comment_at_end_([Values("", " ")] string gap, [ValueSource(nameof(Separators))] string comment)
        {
            var originalCode = @"
public class TestMe
{
    public void DoSomething()
    {
    }

    //" + gap + comment + @"
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test, Combinatorial]
        public void Code_gets_fixed_for_separator_comment_in_method_at_begin_([Values("", " ")] string gap, [ValueSource(nameof(Separators))] string comment)
        {
            var originalCode = @"
public class TestMe
{
    public int DoSomething()
    {
        //" + gap + comment + @"
        return 42;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething()
    {
        return 42;
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test, Combinatorial]
        public void Code_gets_fixed_for_separator_comment_in_method_in_between_([Values("", " ")] string gap, [ValueSource(nameof(Separators))] string comment)
        {
            var originalCode = @"
public class TestMe
{
    public int DoSomething()
    {
        var result = 42;

        //" + gap + comment + @"

        return result;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething()
    {
        var result = 42;
        return result;
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test, Combinatorial]
        public void Code_gets_fixed_for_separator_comment_in_method_at_end_([Values("", " ")] string gap, [ValueSource(nameof(Separators))] string comment)
        {
            var originalCode = @"
public class TestMe
{
    public int DoSomething()
    {
        return 42;

        //" + gap + comment + @"
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething()
    {
        return 42;
    }
}
";

            VerifyCSharpFix(originalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2311_CommentIsSeparatorAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2311_CommentIsSeparatorAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2311_CodeFixProvider();
    }
}