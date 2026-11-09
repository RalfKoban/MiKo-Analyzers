using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_block_spanning_only_single_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_block_with_close_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        {
         }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_block_with_close_brace_indented_more_to_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething()
    {
        {
       }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_block_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
         }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
       }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_with_close_brace_at_same_line_as_last_statement()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        ;}
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        ;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_comment_only_block_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            // some comment
       }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            // some comment
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_comment_only_block_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            // some comment
         }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            // some comment
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_with_directive_before_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            DoSomething();
#if DEBUG
            DoSomething();
#endif
         }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            DoSomething();
#if DEBUG
            DoSomething();
#endif
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_with_directive_before_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            DoSomething();
#if DEBUG
            DoSomething();
#endif
       }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething()
    {
        {
            DoSomething();
#if DEBUG
            DoSomething();
#endif
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6080_CodeFixProvider();
    }
}