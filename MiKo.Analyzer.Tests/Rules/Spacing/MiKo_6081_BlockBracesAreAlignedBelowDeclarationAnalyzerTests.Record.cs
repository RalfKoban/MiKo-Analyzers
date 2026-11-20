using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_record_spanning_only_single_line() => No_issue_is_reported_for(@"
public record TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_record() => No_issue_is_reported_for(@"
public record TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_record_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
namespace Bla
{
    public record TestMe
  {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_record_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public record TestMe
 {
}
");

        [Test]
        public void An_issue_is_reported_for_record_with_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
public record TestMe {
}
");

        [Test]
        public void Code_gets_fixed_for_record_with_open_brace_on_same_line_as_declaration_with_block_comment()
        {
            const string OriginalCode = @"
namespace Bla
{
    public record TestMe /* some comment */ {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public record TestMe /* some comment */
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_record_with_comment_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public record TestMe // some comment
   {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public record TestMe // some comment
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_record_with_comment_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
namespace Bla
{
    public record TestMe // some comment
     {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public record TestMe // some comment
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_record_with_directive_and_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    public record TestMe {
    }
#endif
}
";

            const string FixedCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    public record TestMe
    {
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_record_with_directive_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    public record TestMe
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
    public record TestMe
    {
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_record_with_directive_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    public record TestMe
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
    public record TestMe
    {
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_record_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
namespace Bla
{
    public record TestMe {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public record TestMe
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_record_with_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public record TestMe
   {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public record TestMe
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_record_with_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public record TestMe
 {
}
";

            const string FixedCode = @"
public record TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}
