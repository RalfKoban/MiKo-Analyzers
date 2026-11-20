using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzerTests
    {
        [Test]
        public void No_issue_is_reported_for_switch_statement_spanning_only_single_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i) { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_switch_statement() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
        {
            case 1:
                break;

            default:
                break;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_statement_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
       {
            case 1:
                break;

            default:
                break;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_statement_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
         {
            case 1:
                break;

            default:
                break;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_switch_statement_with_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i) {
            case 1:
                break;

            default:
                break;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_switch_statement_with_open_brace_on_same_line_as_declaration_with_block_comment()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i) /* some comment */ {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i) /* some comment */
        {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_statement_with_comment_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i) // some comment
       {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i) // some comment
        {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_statement_with_comment_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i) // some comment
         {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i) // some comment
        {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_statement_with_directive_and_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
#define FLAG

public class TestMe
{
    public void DoSomething(int i)
    {
#if FLAG
        switch (i) {
            case 1:
                break;

            default:
                break;
        }
#endif
    }
}
";

            const string FixedCode = @"
#define FLAG

public class TestMe
{
    public void DoSomething(int i)
    {
#if FLAG
        switch (i)
        {
            case 1:
                break;

            default:
                break;
        }
#endif
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_statement_with_directive_and_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
#define FLAG

public class TestMe
{
    public void DoSomething(int i)
    {
#if FLAG
        switch (i)
       {
            case 1:
                break;

            default:
                break;
        }
#endif
    }
}
";

            const string FixedCode = @"
#define FLAG

public class TestMe
{
    public void DoSomething(int i)
    {
#if FLAG
        switch (i)
        {
            case 1:
                break;

            default:
                break;
        }
#endif
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_statement_with_directive_and_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
#define FLAG

public class TestMe
{
    public void DoSomething(int i)
    {
#if FLAG
        switch (i)
         {
            case 1:
                break;

            default:
                break;
        }
#endif
    }
}
";

            const string FixedCode = @"
#define FLAG

public class TestMe
{
    public void DoSomething(int i)
    {
#if FLAG
        switch (i)
        {
            case 1:
                break;

            default:
                break;
        }
#endif
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_statement_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i) {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
        {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_statement_with_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
       {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
        {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_switch_statement_with_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
         {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        switch (i)
        {
            case 1:
                break;

            default:
                break;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }
    }
}
