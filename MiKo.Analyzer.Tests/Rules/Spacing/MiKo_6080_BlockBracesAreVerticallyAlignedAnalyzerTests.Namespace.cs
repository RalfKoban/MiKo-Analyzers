using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_namespace_spanning_only_single_line() => No_issue_is_reported_for(@"
namespace TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_namespace() => No_issue_is_reported_for(@"
namespace TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_with_close_brace_indented_more_to_left() => An_issue_is_reported_for(@"
namespace Outer
{
    namespace TestMe
    {
  }
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_with_close_brace_indented_more_to_right() => An_issue_is_reported_for(@"
namespace TestMe
{
 }
");

        [Test]
        public void Code_gets_fixed_for_namespace_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Outer
{
    namespace TestMe
    {
  }
}
";

            const string FixedCode = @"
namespace Outer
{
    namespace TestMe
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
namespace TestMe
{
 }
";

            const string FixedCode = @"
namespace TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_comment_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Outer
{
    namespace TestMe
    {
        // some comment
  }
}
";

            const string FixedCode = @"
namespace Outer
{
    namespace TestMe
    {
        // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_comment_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
namespace TestMe
{
    // some comment
 }
";

            const string FixedCode = @"
namespace TestMe
{
    // some comment
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_directive_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Outer
{
    namespace TestMe
    {
#if DEBUG
        public class Inner { }
#endif
  }
}
";

            const string FixedCode = @"
namespace Outer
{
    namespace TestMe
    {
#if DEBUG
        public class Inner { }
#endif
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_namespace_with_directive_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
namespace TestMe
{
#if DEBUG
    public class Inner { }
#endif
 }
";

            const string FixedCode = @"
namespace TestMe
{
#if DEBUG
    public class Inner { }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}