using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_property_accessor_list_spanning_only_single_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public int Value { get; set; }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_property_accessor_list() => No_issue_is_reported_for(@"
public class TestMe
{
    public int Value
    {
        get;
        set;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_indexer_accessor_list() => No_issue_is_reported_for(@"
public class TestMe
{
    public int this[int index]
    {
        get => 42;
        set { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_event_accessor_list() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler Event
    {
        add { }
        remove { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_accessor_list_with_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
public class TestMe
{
    public int Value {
        get;
        set;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_accessor_list_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public int Value
   {
        get;
        set;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_property_accessor_list_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public int Value
     {
        get;
        set;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_indexer_accessor_list_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public int this[int index]
   {
        get => 42;
        set { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_indexer_accessor_list_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public int this[int index]
     {
        get => 42;
        set { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_event_accessor_list_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler Event
   {
        add { }
        remove { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_event_accessor_list_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public event EventHandler Event
     {
        add { }
        remove { }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_property_accessor_list_with_open_brace_on_same_line_as_declaration_with_block_comment()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Value /* some comment */ {
        get;
        set;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
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
        public void Code_gets_fixed_for_property_accessor_list_with_comment_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Value // some comment
   {
        get;
        set;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
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
        public void Code_gets_fixed_for_property_accessor_list_with_comment_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Value // some comment
     {
        get;
        set;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
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
        public void Code_gets_fixed_for_property_accessor_list_with_directive_and_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
#define FLAG

public class TestMe
{
#if FLAG
    public int Value {
        get;
        set;
    }
#endif
}
";

            const string FixedCode = @"
#define FLAG

public class TestMe
{
#if FLAG
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
        public void Code_gets_fixed_for_property_accessor_list_with_directive_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
#define FLAG

public class TestMe
{
#if FLAG
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

public class TestMe
{
#if FLAG
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
        public void Code_gets_fixed_for_property_accessor_list_with_directive_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
#define FLAG

public class TestMe
{
#if FLAG
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

public class TestMe
{
#if FLAG
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
        public void Code_gets_fixed_for_indexer_accessor_list_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int this[int index] {
        get => 42;
        set { }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int this[int index]
    {
        get => 42;
        set { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_indexer_accessor_list_with_open_brace_on_same_line_as_declaration_with_block_comment()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int this[int index] /* some comment */ {
        get => 42;
        set { }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int this[int index] /* some comment */
    {
        get => 42;
        set { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_indexer_accessor_list_with_comment_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int this[int index] // some comment
     {
        get => 42;
        set { }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int this[int index] // some comment
    {
        get => 42;
        set { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_indexer_accessor_list_with_directive_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
#define FLAG

public class TestMe
{
#if FLAG
    public int this[int index]
   {
        get => 42;
        set { }
    }
#endif
}
";

            const string FixedCode = @"
#define FLAG

public class TestMe
{
#if FLAG
    public int this[int index]
    {
        get => 42;
        set { }
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_accessor_list_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler Event {
        add { }
        remove { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler Event
    {
        add { }
        remove { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_accessor_list_with_open_brace_on_same_line_as_declaration_with_block_comment()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler Event /* some comment */ {
        add { }
        remove { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler Event /* some comment */
    {
        add { }
        remove { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_accessor_list_with_comment_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler Event // some comment
   {
        add { }
        remove { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler Event // some comment
    {
        add { }
        remove { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_accessor_list_with_directive_and_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
#define FLAG

using System;

public class TestMe
{
#if FLAG
    public event EventHandler Event {
        add { }
        remove { }
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
    public event EventHandler Event
    {
        add { }
        remove { }
    }
#endif
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_property_accessor_list_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Value {
        get;
        set;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
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
        public void Code_gets_fixed_for_property_accessor_list_with_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Value
   {
        get;
        set;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
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
        public void Code_gets_fixed_for_property_accessor_list_with_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int Value
     {
        get;
        set;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
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
        public void Code_gets_fixed_for_indexer_accessor_list_with_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int this[int index]
   {
        get => 42;
        set { }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int this[int index]
    {
        get => 42;
        set { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_indexer_accessor_list_with_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int this[int index]
     {
        get => 42;
        set { }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int this[int index]
    {
        get => 42;
        set { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_accessor_list_with_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler Event
   {
        add { }
        remove { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler Event
    {
        add { }
        remove { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_event_accessor_list_with_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
using System;

public class TestMe
{
    public event EventHandler Event
     {
        add { }
        remove { }
    }
}
";

            const string FixedCode = @"
using System;

public class TestMe
{
    public event EventHandler Event
    {
        add { }
        remove { }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}
