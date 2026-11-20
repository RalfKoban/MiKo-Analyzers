using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzerTests
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
        public void An_issue_is_reported_for_interface_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
namespace Bla
{
    public interface ITestMe
  {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_interface_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public interface ITestMe
 {
}
");

        [Test]
        public void An_issue_is_reported_for_interface_with_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
public interface ITestMe {
}
");

        [Test]
        public void Code_gets_fixed_for_interface_with_open_brace_on_same_line_as_declaration_with_block_comment()
        {
            const string OriginalCode = @"
namespace Bla
{
    public interface ITestMe /* some comment */ {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public interface ITestMe /* some comment */
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_comment_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public interface ITestMe // some comment
   {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public interface ITestMe // some comment
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_comment_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
namespace Bla
{
    public interface ITestMe // some comment
     {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public interface ITestMe // some comment
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_directive_and_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    public interface ITestMe {
    }
#endif
}
";

            const string FixedCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    public interface ITestMe
    {
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_directive_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    public interface ITestMe
   {
    }
#endif
}
";

            const string FixedCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    public interface ITestMe
    {
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_directive_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    public interface ITestMe
     {
    }
#endif
}
";

            const string FixedCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    public interface ITestMe
    {
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interface_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
namespace Bla
{
    public interface ITestMe {
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
        public void Code_gets_fixed_for_interface_with_open_brace_indented_more_to_left()
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
        public void Code_gets_fixed_for_interface_with_open_brace_indented_more_to_right()
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
    }
}
