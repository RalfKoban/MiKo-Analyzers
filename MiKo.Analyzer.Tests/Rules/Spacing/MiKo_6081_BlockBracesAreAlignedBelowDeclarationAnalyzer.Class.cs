using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzerTests
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
        public void An_issue_is_reported_for_class_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
  {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
 {
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
public class TestMe {
}
");

        [Test]
        public void Code_gets_fixed_for_class_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe {
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
        public void Code_gets_fixed_for_class_with_open_brace_indented_more_to_left()
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
        public void Code_gets_fixed_for_class_with_open_brace_indented_more_to_right()
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
        public void No_issue_is_reported_for_class_with_attribute_spanning_only_single_line_inclusive_attribute() => No_issue_is_reported_for(@"
using System;

[Serializable] public class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_class_with_attribute_spanning_only_single_line() => No_issue_is_reported_for(@"
using System;

[Serializable]
public class TestMe { }
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_class_with_attribute_on_separate_line() => No_issue_is_reported_for(@"
using System;

[Serializable]
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_class_with_attribute_on_same_line() => No_issue_is_reported_for(@"
using System;

[Serializable] public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_nested_class_with_multiple_attributes() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    [Serializable]
    [Obsolete]
    public class TestMe
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_attribute_and_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    [Serializable]
    public class TestMe
  {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_attribute_and_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    [Serializable]
    public class TestMe
      {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_attribute_and_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    [Serializable]
    public class TestMe {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_class_with_attribute_on_same_line_and_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    [Serializable] public class TestMe
      {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_class_with_attribute_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    [Serializable]
    public class TestMe
  {
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    [Serializable]
    public class TestMe
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_attribute_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    [Serializable]
    public class TestMe
      {
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    [Serializable]
    public class TestMe
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_class_with_attribute_and_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    [Serializable]
    public class TestMe {
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    [Serializable]
    public class TestMe
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}
