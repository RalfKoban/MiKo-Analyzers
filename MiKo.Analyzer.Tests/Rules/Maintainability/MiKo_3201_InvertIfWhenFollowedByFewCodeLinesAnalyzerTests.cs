using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_without_if_statement() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_if_statement_and_else_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return -1;
        }
        else
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_if_statement_and_no_else_block_and_1_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return -1;
        }

        return x * y + z;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_if_statement_and_no_else_block_and_4_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return -1;
        }

        var x = 4;
        var y = 10;
        var z = 2;

        return x * y + z;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_if_statement_and_no_else_block_and_4_following_lines_on_void_method() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
        DoSomethingElse(4);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_throwing_if_statement_and_no_else_block_and_a_single_following_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag) throw new ArgumentException();

        DoSomethingElse(1);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_block_throwing_if_statement_and_no_else_block_and_a_single_following_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            throw new ArgumentException();
        }

        DoSomethingElse(1);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_nested_block_returning_if_statement_and_no_else_block_and_a_single_following_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        if (flag1)
        {
            if (flag2) return;

            DoSomethingElse(1);
        }

        DoSomethingElse(2);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_if_statement_and_no_else_block_and_a_single_following_line() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            return;
        }

        DoSomethingElse(1);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_if_statement_and_no_else_block_and_2_following_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_if_statement_and_no_else_block_and_3_following_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_normal_if_statement_and_no_else_block_and_3_following_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag is false)
        {
            DoSomethingElse(1);
            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_inverted_if_statement_and_no_else_block_and_3_following_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (!flag)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
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
            DoSomethingElse(1);
            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_is_false_if_statement_and_no_else_block_and_3_following_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag is false)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
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
            DoSomethingElse(1);
            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_nullable_is_null_if_statement_and_no_else_block_and_3_following_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool? flag)
    {
        if (flag is null)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool? flag)
    {
        if (flag != null)
        {
            DoSomethingElse(1);
            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_nullable_is_true_if_statement_and_no_else_block_and_3_following_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool? flag)
    {
        if (flag is true)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool? flag)
    {
        if (!(flag is true))
        {
            DoSomethingElse(1);
            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_nullable_is_false_if_statement_and_no_else_block_and_3_following_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool? flag)
    {
        if (flag is false)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool? flag)
    {
        if (!(flag is false))
        {
            DoSomethingElse(1);
            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_null_equality_check_as_if_statement_and_no_else_block_and_3_following_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(object o)
    {
        if (o == null)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(object o)
    {
        if (o != null)
        {
            DoSomethingElse(1);
            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_null_inequality_check_as_if_statement_and_no_else_block_and_3_following_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(object o)
    {
        if (o != null)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(object o)
    {
        if (o is null)
        {
            DoSomethingElse(1);
            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_null_pattern_check_as_if_statement_and_no_else_block_and_3_following_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(object o)
    {
        if (o is null)
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(object o)
    {
        if (o != null)
        {
            DoSomethingElse(1);
            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("== 42", "!= 42")]
        [TestCase("!= 42", "== 42")]
        [TestCase(">= 42", "< 42")]
        [TestCase("<= 42", "> 42")]
        [TestCase("> 42", "<= 42")]
        [TestCase("< 42", ">= 42")]
        public void Code_gets_fixed_for_method_with_number_check_as_if_statement_and_no_else_block_and_3_following_lines(string originalCheck, string fixedCheck)
        {
            var originalCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        if (i " + originalCheck + @")
        {
            return;
        }

        DoSomethingElse(1);
        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            var fixedCode = @"
public class TestMe
{
    public void DoSomething(int i)
    {
        if (i " + fixedCheck + @")
        {
            DoSomethingElse(1);
            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3201_CodeFixProvider();
    }
}