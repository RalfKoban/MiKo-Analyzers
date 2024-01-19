using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3217_InvertIfInsideBlockWhenFollowedBySingleCodeLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_void_method_with_if_statement_and_no_else_block_and_1_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        if (flag)
        {
            return;
        }

        DoSomething(flag);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_loop_and_no_else_block_and_2_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                continue;
            }

            DoSomething(true);
            DoSomething(false);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_loop_and_no_else_block_and_followed_by_another_while_loop() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                continue;
            }

            while (true)
            {
                DoSomething(false);
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_loop_and_no_else_block_and_followed_by_another_do_while_loop() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                continue;
            }

            do
            {
                DoSomething(false);
            } while (true);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_loop_and_no_else_block_and_followed_by_another_for_loop() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                continue;
            }

            for (var i = 0; i < 10; i++)
            {
                DoSomething(false);
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_loop_and_no_else_block_and_followed_by_another_foreach_loop() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMe
{
    public void DoSomething(bool flag, IEnumerable<int> items)
    {
        while(true)
        {
            if (flag)
            {
                continue;
            }

            foreach (var i in items)
            {
                DoSomething(false);
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_loop_and_no_else_block_and_followed_by_try_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                continue;
            }

            try
            {
                DoSomething(false);
            }
            finally
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_loop_and_no_else_block_and_followed_by_using() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag, IDisposable items)
    {
        while(true)
        {
            if (flag)
            {
                continue;
            }

            using (items)
            {
                DoSomething(false);
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_loop_and_no_else_block_and_followed_by_if_block() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        while(true)
        {
            if (flag1)
            {
                continue;
            }

            if (flag2)
            {
                DoSomething(false, true);
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_if_statement_inside_loop_and_no_else_block_and_followed_by_local_function() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(bool flag1)
    {
        while(true)
        {
            if (flag1)
            {
                continue;
            }

            void SomethingLocal(bool flag2)
            {
                DoSomething(false);
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_positive_pattern_condition_if_statement_inside_loop_and_no_else_block_and_1_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag is true)
            {
                continue;
            }

            DoSomething(flag);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_void_method_with_positive_condition_if_statement_inside_loop_and_no_else_block_and_1_following_lines() => No_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                continue;
            }

            DoSomething(flag);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_void_method_with_negative_condition_if_statement_inside_loop_and_no_else_block_and_1_following_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (!flag)
            {
                continue;
            }

            DoSomething(flag);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_void_method_with_negative_pattern_condition_if_statement_inside_loop_and_no_else_block_and_1_following_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag is false)
            {
                continue;
            }

            DoSomething(flag);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_void_method_with_negative_condition_if_statement_inside_loop_and_no_else_block_and_1_preceding_and_1_following_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            DoSomething(flag);

            if (!flag)
            {
                continue;
            }

            DoSomething(flag);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_void_method_with_complex_negative_condition_if_statement_inside_loop_and_no_else_block_and_1_following_lines() => An_issue_is_reported_for(@"
public class TestMe
{
    public void DoSomething(bool flag1, bool flag2)
    {
        while(true)
        {
            if (flag || !flag2)
            {
                continue;
            }

            DoSomething(flag);
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_with_negative_pattern_condition()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag is false)
            {
                continue;
            }

            DoSomething(flag);
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                DoSomething(flag);
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_negative_condition()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (!flag)
            {
                continue;
            }

            DoSomething(flag);
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                DoSomething(flag);
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_comment_inside_if()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (!flag)
            {
                // some comment
                continue;
            }

            DoSomething(flag);
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            // some comment
            if (flag)
            {
                DoSomething(flag);
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_comment_on_follow_up_code()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (!flag)
            {
                continue;
            }

            // some comment
            DoSomething(flag);
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                // some comment
                DoSomething(flag);
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_negative_condition_and_preceding_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            DoSomething(false);

            if (!flag)
            {
                continue;
            }

            DoSomething(flag);
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            DoSomething(false);

            if (flag)
            {
                DoSomething(flag);
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_negative_condition_and_continue_without_block_on_same_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (!flag) continue;

            DoSomething(flag);
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                DoSomething(flag);
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_negative_condition_and_continue_without_block_on_separate_line()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (!flag)
                continue;

            DoSomething(flag);
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (flag)
            {
                DoSomething(flag);
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_negative_condition_and_continue_without_block_on_same_line_with_comment_behind()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (!flag) continue; // some comment

            DoSomething(flag);
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            // some comment
            if (flag)
            {
                DoSomething(flag);
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_negative_condition_and_continue_without_block_on_separate_line_with_comment_before()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (!flag)
                // some comment
                continue;

            DoSomething(flag);
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            // some comment
            if (flag)
            {
                DoSomething(flag);
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_with_negative_condition_and_continue_without_block_on_separate_line_with_comment_behind()
        {
            const string OriginalCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            if (!flag)
                continue; // some comment

            DoSomething(flag);
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public void DoSomething(bool flag)
    {
        while(true)
        {
            // some comment
            if (flag)
            {
                DoSomething(flag);
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3217_InvertIfInsideBlockWhenFollowedBySingleCodeLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3217_InvertIfInsideBlockWhenFollowedBySingleCodeLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3217_CodeFixProvider();
    }
}