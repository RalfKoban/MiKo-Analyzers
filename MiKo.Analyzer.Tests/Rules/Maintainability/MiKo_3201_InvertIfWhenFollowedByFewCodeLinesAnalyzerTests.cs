using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_void_method_without_if_statement() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething()
    {
        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_void_method_with_if_statement_and_else_block() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_non_void_method_with_if_statement_and_no_else_block_and_1_following_lines() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_non_void_method_with_if_statement_and_no_else_block_and_4_following_lines() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_void_method_with_if_statement_and_no_else_block_and_4_following_lines_on_void_method() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_void_method_with_throwing_if_statement_and_no_else_block_and_a_single_following_line() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_void_method_with_block_throwing_if_statement_and_no_else_block_and_a_single_following_line() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_void_method_with_nested_block_returning_if_statement_and_no_else_block_and_a_single_following_line() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_void_method_with_if_statement_containing_some_lines_that_has_no_follow_up_code() => No_issue_is_reported_for(@"
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
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_containing_some_lines_and_no_else_block_and_1_following_lines() => No_issue_is_reported_for(@"
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

        DoSomethingElse(4);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_containing_some_lines_and_no_else_block_and_4_following_lines() => No_issue_is_reported_for(@"
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

        DoSomethingElse(4);
        DoSomethingElse(5);
        DoSomethingElse(6);
        DoSomethingElse(7);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_containing_a_method_call_and_a_return_and_no_else_block_and_2_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            DoSomethingElse(1);
            return;
        }

        DoSomethingElse(2);
        DoSomethingElse(3);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_a_while_loop_returns_and_has_2_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while (true)
        {
            if (flag)
            {
                return;
            }

            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_a_do_while_loop_returns_and_has_2_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        do
        {
            if (flag)
            {
                return;
            }

            DoSomethingElse(2);
            DoSomethingElse(3);
        }
        while (true);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_a_for_loop_returns_and_has_2_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        for (var i = 0; i < 10; i++)
        {
            if (flag)
            {
                return;
            }

            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_a_foreach_loop_returns_and_has_2_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag, int[] values)
    {
        foreach (var value in values)
        {
            if (flag)
            {
                return;
            }

            DoSomethingElse(2);
            DoSomethingElse(3);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_nested_if_statement_inside_a_foreach_loop_returns_and_has_2_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag, int[] values)
    {
        foreach (var value in values)
        {
            if (value == 1)
            {
                if (flag)
                {
                    return;
                }

                DoSomethingElse(2);
                DoSomethingElse(3);
            }
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_is_followed_by_a_do_loop() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            return;
        }

        do
        {
            DoSomethingElse(1);
        } while (true);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_is_followed_by_a_while_loop() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            return;
        }

        while (true)
        {
            DoSomethingElse(1);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_is_followed_by_a_for_loop() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            return;
        }

        for (var i = 0; i < 10; i++)
        {
            DoSomethingElse(i);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_is_followed_by_a_foreach_loop() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag, int[] values)
    {
        if (flag)
        {
            return;
        }

        foreach (var value in values)
        {
            DoSomethingElse(value);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_is_followed_by_a_switch() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag, int value)
    {
        if (flag)
        {
            return;
        }

        switch (value)
        {
            case 0: return;
            case 1: return;
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_is_followed_by_another_if() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag, int value)
    {
        if (flag)
        {
            return;
        }

        if (value == 0)
        {
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_is_followed_by_using() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag, IDisposable value)
    {
        if (flag)
        {
            return;
        }

        using (value)
        {
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_is_followed_by_try_finally() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag, IDisposable value)
    {
        if (flag)
        {
            return;
        }

        try
        {
        }
        finally
        {
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_is_followed_by_local_function() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag, IDisposable value)
    {
        if (flag)
        {
            return;
        }

        void MyFunction()
        {
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_and_comment_above_return_and_no_else_block_and_a_single_following_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            // for some reason
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
        public void No_issue_is_reported_for_void_method_with_if_statement_and_comment_behind_return_and_no_else_block_and_a_single_following_line() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            return; // for some reason
        }

        DoSomethingElse(1);
    }

    private void DoSomethingElse(int i)
    {
    }
}
");

        [Test]
        public void An_issue_is_reported_for_void_method_with_if_statement_and_no_else_block_and_a_single_following_line() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_void_method_with_if_statement_and_no_else_block_and_2_following_lines() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_void_method_with_if_statement_and_no_else_block_and_3_following_lines() => An_issue_is_reported_for(@"
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
        public void Code_gets_fixed_for_void_method_with_normal_if_statement_and_no_else_block_and_3_following_lines()
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
        public void Code_gets_fixed_for_void_method_with_inverted_if_statement_and_no_else_block_and_3_following_lines()
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
        public void Code_gets_fixed_for_void_method_with_is_false_if_statement_and_no_else_block_and_3_following_lines()
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
        public void Code_gets_fixed_for_void_method_with_nullable_is_null_if_statement_and_no_else_block_and_3_following_lines()
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
        public void Code_gets_fixed_for_void_method_with_nullable_is_true_if_statement_and_no_else_block_and_3_following_lines()
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
        public void Code_gets_fixed_for_void_method_with_nullable_is_false_if_statement_and_no_else_block_and_3_following_lines()
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
        public void Code_gets_fixed_for_void_method_with_null_equality_check_as_if_statement_and_no_else_block_and_3_following_lines()
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
        public void Code_gets_fixed_for_void_method_with_null_inequality_check_as_if_statement_and_no_else_block_and_3_following_lines()
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
        public void Code_gets_fixed_for_void_method_with_null_pattern_check_as_if_statement_and_no_else_block_and_3_following_lines()
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
        public void Code_gets_fixed_for_void_method_with_number_check_as_if_statement_and_no_else_block_and_3_following_lines(string originalCheck, string fixedCheck)
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

        [Test]
        public void Code_gets_fixed_with_comments()
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

        // some comment
        DoSomethingElse(1);

        // some other comment
        DoSomethingElse(2);
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
            // some comment
            DoSomethingElse(1);

            // some other comment
            DoSomethingElse(2);
        }
    }

    private void DoSomethingElse(int i)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3201_InvertIfWhenFollowedByFewCodeLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3201_CodeFixProvider();
    }
}