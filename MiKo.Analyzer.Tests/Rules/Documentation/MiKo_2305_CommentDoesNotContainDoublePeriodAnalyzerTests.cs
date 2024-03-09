using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_comment_with_single_period() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        // some comment.
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multi_line_comment_with_single_period() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        // some comment.
        // more comment.
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_triple_period() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        // some comment...
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multi_line_comment_with_triple_period() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        // some comment...
        // more comment...
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_relative_file_path() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        // some path/to/../relative/file.txt
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multi_line_comment_with_relative_file_path() => No_issue_is_reported_for(@"
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        // some path/to/../relative/file.txt
        // some other\path\to\..\relative\file.txt
    }
}
");

        [Test]
        public void An_issue_is_reported_for_comment_with_double_period() => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment..
    }
}
");

        [Test]
        public void An_issue_is_reported_for_multi_line_comment_with_double_period() => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment..
        // whatever it takes
    }
}
");

        [Test]
        public void Code_gets_fixed_for_single_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment..
        DoSomething();
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment.
        DoSomething();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_single_line_with_double_dots_in_middle_of_comment()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment.. further more
        DoSomething();
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment. further more
        DoSomething();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment..
        // some more comment..
        // even some more comment..
        DoSomething();
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment.
        // some more comment.
        // even some more comment.
        DoSomething();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_lines_with_etc()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment..
        // some more comment, etc..
        // even some more comment..
        DoSomething();
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment.
        // some more comment, etc.
        // even some more comment.
        DoSomething();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_lines_with_triple_dot()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment..
        // some more comment ...
        // even some more comment..
        DoSomething();
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment.
        // some more comment ...
        // even some more comment.
        DoSomething();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2305_CommentDoesNotContainDoublePeriodAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2305_CodeFixProvider();
    }
}