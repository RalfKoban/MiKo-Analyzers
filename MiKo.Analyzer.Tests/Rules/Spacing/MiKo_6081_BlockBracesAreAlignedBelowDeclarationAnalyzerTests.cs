using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed partial class MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_block_spanning_only_single_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_block_of_if_statement() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_vertically_aligned_block_of_else_if_statement() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
        else if (flag is false)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_block_of_if_statement_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
       {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_block_of_if_statement_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
         {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_block_of_else_if_statement_with_open_brace_indented_more_to_left() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
        else if (flag is false)
       {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_block_of_else_if_statement_with_open_brace_indented_more_to_right() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
        else if (flag is false)
         {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_block_of_if_statement_with_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag) {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_block_of_else_if_statement_with_open_brace_on_same_line_as_declaration() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
        else if (flag is false) {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_block_of_if_statement_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag) {
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_of_else_if_statement_with_open_brace_on_same_line_as_declaration()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
        else if (flag is false) {
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
        else if (flag is false)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_of_if_statement_with_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
       {
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_of_if_statement_with_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
         {
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_block_of_else_if_statement_with_open_brace_indented_more_to_left()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
        else if (flag is false)
       {
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
        else if (flag is false)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed__for_block_of_else_if_statement_with_open_brace_indented_more_to_right()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
        else if (flag is false)
         {
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
        }
        else if (flag is false)
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6081_BlockBracesAreAlignedBelowDeclarationAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6081_CodeFixProvider();
    }
}