using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_property_with_attribute_and_accessor_list_on_same_line_as_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    [Obsolete]
    public int Value { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_property_with_attribute_on_same_line_and_accessor_list_on_same_line_as_property() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    [Obsolete] public int Value { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_property_with_attribute_on_separate_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    [Obsolete]
    public int Value
    {
        get;
        set;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_property_with_attribute_on_same_line() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    [Obsolete] public int Value
    {
        get;
        set;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_property_with_multiple_attributes() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    [Obsolete]
    [CLSCompliant(true)]
    public int Value
    {
        get;
        set;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_attribute_and_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    [Obsolete]
    public int Value
  {
        get;
        set;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_attribute_and_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    [Obsolete]
    public int Value
      {
        get;
        set;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_attribute_and_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    [Obsolete]
    public int Value {
        get;
        set;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_with_attribute_on_same_line_and_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    [Obsolete] public int Value
      {
        get;
        set;
    }
}
");

        [Test]
        public void Code_gets_fixed_for_property_with_attribute_on_same_line_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    [Obsolete] public int Value
      {
        get;
        set;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    [Obsolete] public int Value
    {
        get;
        set;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_attribute_and_open_brace_on_same_line_as_declaration_with_block_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value /* some comment */ {
        get;
        set;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value /* some comment */
    {
        get;
        set;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_attribute_and_comment_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value // some comment
   {
        get;
        set;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value // some comment
    {
        get;
        set;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_attribute_and_comment_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value // some comment
     {
        get;
        set;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value // some comment
    {
        get;
        set;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_attribute_and_directive_and_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
#define FLAG

using System;

public class TestMe
{
#if FLAG
    [Obsolete]
    public int Value {
        get;
        set;
    }
#endif
}
";

            const string FixedCode = @"
#define FLAG

using System;

public class TestMe
{
#if FLAG
    [Obsolete]
    public int Value
    {
        get;
        set;
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_attribute_and_directive_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
#define FLAG

using System;

public class TestMe
{
#if FLAG
    [Obsolete]
    public int Value
   {
        get;
        set;
    }
#endif
}
";

            const string FixedCode = @"
#define FLAG

using System;

public class TestMe
{
#if FLAG
    [Obsolete]
    public int Value
    {
        get;
        set;
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_attribute_and_directive_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
#define FLAG

using System;

public class TestMe
{
#if FLAG
    [Obsolete]
    public int Value
     {
        get;
        set;
    }
#endif
}
";

            const string FixedCode = @"
#define FLAG

using System;

public class TestMe
{
#if FLAG
    [Obsolete]
    public int Value
    {
        get;
        set;
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_attribute_and_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value {
        get;
        set;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value
    {
        get;
        set;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_attribute_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value
  {
        get;
        set;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value
    {
        get;
        set;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_with_attribute_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value
      {
        get;
        set;
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    [Obsolete]
    public int Value
    {
        get;
        set;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}
