using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_extension_spanning_only_single_line() => No_issue_is_reported_for(@"
public static class TestMe
{
    extension(string value) { }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_extension() => No_issue_is_reported_for(@"
public static class TestMe
{
    extension(string value)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_extension_with_close_brace_indented_more_to_left() => An_issue_is_reported_for(@"
public static class TestMe
{
    extension(string value)
    {
  }
}
");

        [Test]
        public void An_issue_is_reported_for_extension_with_close_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public static class TestMe
{
    extension(string value)
    {
     }
}
");

        [Test]
        public void Code_gets_fixed_for_extension_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public static class TestMe
{
    extension(string value)
    {
  }
}
";

            const string FixedCode = @"
public static class TestMe
{
    extension(string value)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_extension_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public static class TestMe
{
    extension(string value)
    {
     }
}
";

            const string FixedCode = @"
public static class TestMe
{
    extension(string value)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_extension_with_close_brace_on_same_line_as_last_member()
        {
            const string OriginalCode = @"
public static class TestMe
{
    extension(string value)
    {
        public bool IsEmpty => value.Length == 0; }
}
";

            const string FixedCode = @"
public static class TestMe
{
    extension(string value)
    {
        public bool IsEmpty => value.Length == 0;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_extension_with_close_brace_on_same_line_as_last_member_with_block_comment()
        {
            const string OriginalCode = @"
public static class TestMe
{
    extension(string value)
    {
        public bool IsEmpty => value.Length == 0; /* some comment */ }
}
";

            const string FixedCode = @"
public static class TestMe
{
    extension(string value)
    {
        public bool IsEmpty => value.Length == 0; /* some comment */
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_extension_with_comment_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public static class TestMe
{
    extension(string value)
    {
        // some comment
  }
}
";

            const string FixedCode = @"
public static class TestMe
{
    extension(string value)
    {
        // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_extension_with_comment_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public static class TestMe
{
    extension(string value)
    {
        // some comment
     }
}
";

            const string FixedCode = @"
public static class TestMe
{
    extension(string value)
    {
        // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_extension_with_directive_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public static class TestMe
{
    extension(string value)
    {
#if DEBUG
        public bool IsEmpty => value.Length == 0;
#endif
  }
}
";

            const string FixedCode = @"
public static class TestMe
{
    extension(string value)
    {
#if DEBUG
        public bool IsEmpty => value.Length == 0;
#endif
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_extension_with_directive_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public static class TestMe
{
    extension(string value)
    {
#if DEBUG
        public bool IsEmpty => value.Length == 0;
#endif
     }
}
";

            const string FixedCode = @"
public static class TestMe
{
    extension(string value)
    {
#if DEBUG
        public bool IsEmpty => value.Length == 0;
#endif
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}