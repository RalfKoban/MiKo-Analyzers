using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_struct_spanning_only_single_line() => No_issue_is_reported_for(@"
public struct TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_struct() => No_issue_is_reported_for(@"
public struct TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_struct_with_close_brace_indented_more_to_left() => An_issue_is_reported_for(@"
namespace Bla
{
    public struct TestMe
    {
  }
}
");

        [Test]
        public void An_issue_is_reported_for_struct_with_close_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public struct TestMe
{
 }
");

        [Test]
        public void Code_gets_fixed_for_struct_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public struct TestMe
    {
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public struct TestMe
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_struct_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public struct TestMe
{
 }
";

            const string FixedCode = @"
public struct TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_struct_with_close_brace_on_same_line_as_last_member()
        {
            const string OriginalCode = @"
public struct TestMe
{
    private int _field; }
";

            const string FixedCode = @"
public struct TestMe
{
    private int _field;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_struct_with_close_brace_on_same_line_as_last_member_with_block_comment()
        {
            const string OriginalCode = @"
public struct TestMe
{
    private int _field; /* some comment */ }
";

            const string FixedCode = @"
public struct TestMe
{
    private int _field; /* some comment */
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_struct_with_comment_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public struct TestMe
    {
        // some comment
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public struct TestMe
    {
        // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_struct_with_comment_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public struct TestMe
{
    // some comment
 }
";

            const string FixedCode = @"
public struct TestMe
{
    // some comment
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_struct_with_directive_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public struct TestMe
    {
#if DEBUG
        private int _field;
#endif
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public struct TestMe
    {
#if DEBUG
        private int _field;
#endif
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_struct_with_directive_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public struct TestMe
{
#if DEBUG
    private int _field;
#endif
 }
";

            const string FixedCode = @"
public struct TestMe
{
#if DEBUG
    private int _field;
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}