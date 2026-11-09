using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_interface_spanning_only_single_line() => No_issue_is_reported_for(@"
public interface ITestMe { }
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_interface() => No_issue_is_reported_for(@"
public interface ITestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_interface_with_close_brace_indented_more_to_left() => An_issue_is_reported_for(@"
namespace Bla
{
    public interface ITestMe
    {
  }
}
");

        [Test]
        public void An_issue_is_reported_for_interface_with_close_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public interface ITestMe
{
 }
");

        [Test]
        public void Code_gets_fixed_for_interface_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public interface ITestMe
    {
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public interface ITestMe
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public interface ITestMe
{
 }
";

            const string FixedCode = @"
public interface ITestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_close_brace_on_same_line_as_last_member()
        {
            const string OriginalCode = @"
public interface ITestMe
{
    void DoSomething(); }
";

            const string FixedCode = @"
public interface ITestMe
{
    void DoSomething();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_close_brace_on_same_line_as_last_member_with_block_comment()
        {
            const string OriginalCode = @"
public interface ITestMe
{
    void DoSomething(); /* some comment */ }
";

            const string FixedCode = @"
public interface ITestMe
{
    void DoSomething(); /* some comment */
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_comment_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public interface ITestMe
    {
        // some comment
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public interface ITestMe
    {
        // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_comment_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public interface ITestMe
{
    // some comment
 }
";

            const string FixedCode = @"
public interface ITestMe
{
    // some comment
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_directive_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public interface ITestMe
    {
#if DEBUG
        void DoSomething();
#endif
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public interface ITestMe
    {
#if DEBUG
        void DoSomething();
#endif
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_directive_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public interface ITestMe
{
#if DEBUG
    void DoSomething();
#endif
 }
";

            const string FixedCode = @"
public interface ITestMe
{
#if DEBUG
    void DoSomething();
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}