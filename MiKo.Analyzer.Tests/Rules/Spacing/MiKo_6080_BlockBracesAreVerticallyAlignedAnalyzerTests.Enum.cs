using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_enum_spanning_only_single_line() => No_issue_is_reported_for(@"
public enum TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_enum() => No_issue_is_reported_for(@"
public enum TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_enum_with_close_brace_indented_more_to_left() => An_issue_is_reported_for(@"
namespace Bla
{
    public enum TestMe
    {
  }
}
");

        [Test]
        public void An_issue_is_reported_for_enum_with_close_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public enum TestMe
{
 }
");

        [Test]
        public void Code_gets_fixed_for_enum_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public enum TestMe
    {
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public enum TestMe
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public enum TestMe
{
 }
";

            const string FixedCode = @"
public enum TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_with_close_brace_on_same_line_as_last_enum_member()
        {
            const string OriginalCode = @"
public enum TestMe
{
    None }
";

            const string FixedCode = @"
public enum TestMe
{
    None
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_with_close_brace_on_same_line_as_last_enum_member_comma()
        {
            const string OriginalCode = @"
public enum TestMe
{
    None, }
";

            const string FixedCode = @"
public enum TestMe
{
    None,
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_with_close_brace_on_same_line_as_last_enum_member_comma_with_block_comment()
        {
            const string OriginalCode = @"
public enum TestMe
{
    None, /* some comment */ }
";

            const string FixedCode = @"
public enum TestMe
{
    None, /* some comment */
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_with_close_brace_on_same_line_as_last_enum_member_with_block_comment()
        {
            const string OriginalCode = @"
public enum TestMe
{
    None /* some comment */ }
";

            const string FixedCode = @"
public enum TestMe
{
    None /* some comment */
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_with_comment_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public enum TestMe
    {
        // some comment
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public enum TestMe
    {
        // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_with_comment_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public enum TestMe
{
    // some comment
 }
";

            const string FixedCode = @"
public enum TestMe
{
    // some comment
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_with_directive_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public enum TestMe
    {
#if DEBUG
        None,
#endif
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public enum TestMe
    {
#if DEBUG
        None,
#endif
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_enum_with_directive_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public enum TestMe
{
#if DEBUG
    None,
#endif
 }
";

            const string FixedCode = @"
public enum TestMe
{
#if DEBUG
    None,
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}