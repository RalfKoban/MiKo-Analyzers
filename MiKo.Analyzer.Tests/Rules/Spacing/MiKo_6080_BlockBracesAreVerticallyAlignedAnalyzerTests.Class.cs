using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6080_BlockBracesAreVerticallyAlignedAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_class_spanning_only_single_line() => No_issue_is_reported_for(@"
public class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_close_brace_indented_more_to_left() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
  }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_close_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
 }
");

        [Test]
        public void Code_gets_fixed_for_class_with_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
 }
";

            const string FixedCode = @"
public class TestMe
{
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_comment_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        // some comment
  }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_comment_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    // some comment
 }
";

            const string FixedCode = @"
public class TestMe
{
    // some comment
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_directive_and_close_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
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
    public class TestMe
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
        public void Code_gets_fixed_for_class_with_directive_and_close_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
#if DEBUG
    private int _field;
#endif
 }
";

            const string FixedCode = @"
public class TestMe
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