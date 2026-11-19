using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzerTests
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
        public void An_issue_is_reported_for_enum_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
namespace Bla
{
    public enum TestMe
  {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_enum_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public enum TestMe
 {
}
");

        [Test]
        public void An_issue_is_reported_for_enum_with_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
public enum TestMe {
}
");

        [Test]
        public void Code_gets_fixed_for_enum_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
namespace Bla
{
    public enum TestMe {
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
        public void Code_gets_fixed_for_enum_with_open_brace_indented_more_to_left()
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
        public void Code_gets_fixed_for_enum_with_open_brace_indented_more_to_right()
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
    }
}