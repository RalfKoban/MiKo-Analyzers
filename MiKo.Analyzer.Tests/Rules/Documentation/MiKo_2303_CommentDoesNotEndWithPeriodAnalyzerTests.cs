using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2303_CommentDoesNotEndWithPeriodAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_comment_without_period() => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_comment_with_period() => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment.
    }
}
");

        [Test]
        public void An_issue_is_reported_for_comment_with_period_and_no_starting_space() => An_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        //some comment.
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_multi_line_comment_with_period() => An_issue_is_reported_for(3, @"

public class TestMe
{
    public void DoSomething()
    {
        // some comment.
        // another comment.
        // final comment.
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_triple_period() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment...
    }
}
");

        [Test]
        public void No_issue_is_reported_for_comment_with_etc_period() => No_issue_is_reported_for(@"

public class TestMe
{
    public void DoSomething()
    {
        // some comment, etc.
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
        // some comment.
        DoSomething();
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment
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
        // some comment.
        // some more comment.
        // even some more comment.
        DoSomething();
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment
        // some more comment
        // even some more comment
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
        // some comment.
        // some more comment, etc.
        // even some more comment.
        DoSomething();
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment
        // some more comment, etc.
        // even some more comment
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
        // some comment.
        // some more comment ...
        // even some more comment.
        DoSomething();
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        // some comment
        // some more comment ...
        // even some more comment
        DoSomething();
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2303_CommentDoesNotEndWithPeriodAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2303_CommentDoesNotEndWithPeriodAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2303_CodeFixProvider();
    }
}