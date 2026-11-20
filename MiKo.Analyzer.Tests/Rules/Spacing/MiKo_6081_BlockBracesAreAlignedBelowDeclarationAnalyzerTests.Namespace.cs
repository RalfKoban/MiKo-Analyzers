using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_namespace_spanning_only_single_line() => No_issue_is_reported_for(@"
namespace Bla { }
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_namespace() => No_issue_is_reported_for(@"
namespace Bla
{
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
namespace Bla
{
    namespace Blubb
  {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
namespace Bla
 {
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_with_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
namespace Bla {
}
");

        [Test]
        public void Code_gets_fixed_for_namespace_with_open_brace_on_same_line_as_declaration_with_block_comment()
        {
            const string OriginalCode = @"
namespace Bla
{
    namespace Blubb /* some comment */ {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    namespace Blubb /* some comment */
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_comment_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    namespace Blubb // some comment
   {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    namespace Blubb // some comment
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_comment_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
namespace Bla
{
    namespace Blubb // some comment
     {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    namespace Blubb // some comment
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_directive_and_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    namespace Blubb {
    }
#endif
}
";

            const string FixedCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    namespace Blubb
    {
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_directive_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    namespace Blubb
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
    namespace Blubb
    {
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_directive_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
#define FLAG

namespace Bla
{
#if FLAG
    namespace Blubb
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
    namespace Blubb
    {
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
namespace Bla
{
    namespace Blubb {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    namespace Blubb
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    namespace Blubb
   {
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    namespace Blubb
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
namespace Bla
 {
}
";

            const string FixedCode = @"
namespace Bla
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}
