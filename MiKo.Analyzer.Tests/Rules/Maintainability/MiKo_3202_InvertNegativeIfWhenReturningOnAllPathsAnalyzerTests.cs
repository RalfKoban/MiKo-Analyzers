using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3202_InvertNegativeIfWhenReturningOnAllPathsAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_with_positive_if_statement_that_does_not_return_and_no_else_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_negative_if_statement_that_returns_and_no_else_block_but_follow_up_code() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            return 42;
        }

        DoSomething(flag);
        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_negative_if_statement_that_has_a_call_before_it_returns_and_no_else_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            DoSomething(flag);
            return 42;
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_negative_if_statement_that_returns_and_an_else_block_with_code() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            return 42;
        }
        else
        {
            DoSomething(flag);
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_negative_if_statement_that_does_not_return_and_no_else_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_is_false_if_statement_that_does_not_return_and_no_else_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag is false)
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_is_false_if_statement_that_has_a_call_before_it_returns_and_no_else_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag is false)
        {
            DoSomething(flag);
            return -1;
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_is_false_if_statement_that_returns_and_no_else_block_but_follow_up_code() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag is false)
        {
            return -1;
        }

        DoSomething(flag);
        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_is_true_if_statement_that_does_not_return_and_no_else_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag is true)
        {
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_negative_else_if_statement_that_returns_and_else_block_that_returns() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(int i)
    {
        if (i == 0)
        {
        }
        else if (i != 5)
        {
            return 42;
        }

        return -1;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_positive_if_statement_that_returns_and_else_block_that_returns() => No_issue_is_reported_for(@"
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
            return 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_positive_if_statement_that_returns_and_else_block_that_returns_but_no_braces() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
            return -1;
        else
            return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parenthesized_positive_if_statement_that_returns_and_else_block_that_returns() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if ((flag))
        {
            return -1;
        }
        else
        {
            return 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_positive_conditional_statement_inside_return() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag ? -1 : 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_is_true_conditional_statement_inside_return() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag is true ? -1 : 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parenthesized_is_true_conditional_statement_inside_return() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return (flag is true) ? -1 : 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_positive_conditional_statement_inside_parenthesized_return() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return (flag ? -1 : 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_is_true_conditional_statement_inside_parenthesized_return() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return (flag is true ? -1 : 42);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parenthesized_ORed_conditional_statement_inside_return() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag1, bool flag2)
    {
        return (flag1 || flag2) ? -1 : 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_parenthesized_ANDed_conditional_statement_inside_return() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag1, bool flag2)
    {
        return (flag1 && flag2) ? -1 : 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_expression_body_with_positive_conditional_statement_inside_return() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag) => flag ? -1 : 42;
}
");

        [Test]
        public void No_issue_is_reported_for_method_expression_body_with_is_true_conditional_statement_inside_return() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag) => flag is true ? -1 : 42;
}
");

        [Test]
        public void No_issue_is_reported_for_method_when_more_conditions_are_positive_than_negative() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag1, bool flag2, bool flag3, bool flag4)
    {
        if (flag1 || flag2 || !flag3 || flag4)
        {
            return 42;
        }

        return -1;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_when_half_of_conditions_are_positive() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag1, bool flag2, bool flag3, bool flag4)
    {
        if (flag1 || flag2 || !flag3 || !flag4)
        {
            return 42;
        }

        return -1;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_negative_if_statement_that_returns_and_follow_up_code_that_returns() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            return -1;
        }

        return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_negative_if_statement_that_returns_and_else_block_that_returns() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            return -1;
        }
        else
        {
            return 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_negative_if_statement_that_returns_and_else_block_that_returns_but_no_braces() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
            return -1;
        else
            return 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_negative_parenthesized_if_statement_that_returns_and_else_block_that_returns() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if ((!flag))
        {
            return -1;
        }
        else
        {
            return 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_is_false_if_statement_that_returns_and_else_block_that_returns() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag is false)
        {
            return -1;
        }
        else
        {
            return 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_parenthesized_is_false_if_statement_that_returns_and_else_block_that_returns() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if ((flag is false))
        {
            return -1;
        }
        else
        {
            return 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_negative_conditional_statement_inside_return() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return !flag ? -1 : 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_negative_parenthesized_conditional_statement_inside_return() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return (!flag) ? -1 : 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_negative_conditional_statement_inside_parenthesized_return() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return (!flag ? -1 : 42);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_is_false_conditional_statement_inside_return() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag is false ? -1 : 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_parenthesized_is_false_conditional_statement_inside_return() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return (flag is false) ? -1 : 42;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_expression_body_with_negative_parenthesized_conditional_statement_inside_return() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag) => (!flag) ? -1 : 42;
}
");

        [Test]
        public void An_issue_is_reported_for_method_expression_body_with_negative_conditional_statement_inside_return() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag) => !flag ? -1 : 42;
}
");

        [Test]
        public void An_issue_is_reported_for_method_expression_body_with_is_false_conditional_statement_inside_return() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag) => flag is false ? -1 : 42;
}
");

        [Test]
        public void An_issue_is_reported_for_method_expression_body_with_parenthesized_is_false_conditional_statement_inside_return() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag) => (flag is false) ? -1 : 42;
}
");

        [Test]
        public void An_issue_is_reported_for_method_when_more_conditions_are_negative_than_positive() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag1, bool flag2, bool flag3, bool flag4)
    {
        if (!flag1 && !flag2 && flag3 && !flag4)
        {
            return 42;
        }

        return -1;
    }
}
");

        [Test]
        public void Code_gets_fixed_for_method_with_negative_if_statement_that_returns_and_follow_up_code_that_returns()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            return -1;
        }

        return 42;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return 42;
        }

        return -1;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_negative_if_statement_that_returns_and_else_block_that_returns()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            return -1;
        }
        else
        {
            return 42;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return 42;
        }
        else
        {
            return -1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_negative_if_statement_that_returns_and_else_block_that_returns_but_no_braces()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
            return -1;
        else
            return 42;
    }
}
";
            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
            return 42;
        else
            return -1;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_negative_parenthesized_if_statement_that_returns_and_else_block_that_returns()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if ((!flag))
        {
            return -1;
        }
        else
        {
            return 42;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return 42;
        }
        else
        {
            return -1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_is_false_if_statement_that_returns_and_else_block_that_returns()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag is false)
        {
            return -1;
        }
        else
        {
            return 42;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return 42;
        }
        else
        {
            return -1;
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parenthesized_is_false_if_statement_that_returns_and_else_block_that_returns()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if ((flag is false))
        {
            return -1;
        }
        else
        {
            return 42;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return 42;
        }
        else
        {
            return -1;
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_negative_conditional_statement_inside_return()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return !flag ? -1 : 42;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag ? 42 : -1;
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_negative_parenthesized_conditional_statement_inside_return()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return (!flag) ? -1 : 42;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag ? 42 : -1;
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_negative_conditional_statement_inside_parenthesized_return()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return (!flag ? -1 : 42);
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag ? 42 : -1;
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_is_false_conditional_statement_inside_return()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag is false ? -1 : 42;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag ? 42 : -1;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_parenthesized_is_false_conditional_statement_inside_return()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return (flag is false) ? -1 : 42;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag ? 42 : -1;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_expression_body_with_negative_parenthesized_conditional_statement_inside_return()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag) => (!flag) ? -1 : 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag) => flag ? 42 : -1;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_expression_body_with_negative_conditional_statement_inside_return()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag) => !flag ? -1 : 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag) => flag ? 42 : -1;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_expression_body_with_is_false_conditional_statement_inside_return()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag) => flag is false ? -1 : 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag) => flag ? 42 : -1;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_expression_body_with_parenthesized_is_false_conditional_statement_inside_return()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag) => (flag is false) ? -1 : 42;
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag) => flag ? 42 : -1;
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_conditional_statement_inside_return_that_spans_multiple_lines()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return !flag
                ? true
                : false;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag
                ? false
                : true;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_conditional_statement_inside_return_that_spans_multiple_lines_and_contains_comment_in_true_clause()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return !flag
                ? true // some comment
                : false;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag
                ? false
                : true; // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_conditional_statement_inside_return_that_spans_multiple_lines_and_contains_comment_in_false_clause()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return !flag
                ? true
                : false; // some comment
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag
                ? false // some comment
                : true;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_conditional_statement_inside_return_that_spans_multiple_lines_and_contains_comment_in_both_clauses()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return !flag
                ? true // comment 1
                : false; // comment 2
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag
                ? false // comment 2
                : true; // comment 1
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_conditional_statement_inside_return_that_spans_single_and_contains_comment_at_line_end()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return !flag ? true : false; // some comment
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        return flag ? false : true; // some comment
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_if_statement_and_no_else_block_and_comment_in_follow_up_code()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
            return true;

        // some comment
        return false;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            // some comment
            return false;
        }

        return true;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_if_statement_and_no_else_block_and_comment_in_condition()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            // some comment
            return true;
        }

        return false;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return false;
        }

        // some comment
        return true;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_if_statement_and_else_block_and_comments_in_both()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            // some comment 1
            return true;
        }
        else
        {
            // some comment 2
            return false;
        }
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            // some comment 2
            return false;
        }
        else
        {
            // some comment 1
            return true;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_when_more_conditions_are_negative_than_positive()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag1, bool flag2, bool flag3, bool flag4)
    {
        if (!flag1 && !flag2 && flag3 && !flag4)
        {
            return -1;
        }

        return 42;
    }
}
";

            const string FixedCode = @"
public class TestMe
{
    public int DoSomething(bool flag1, bool flag2, bool flag3, bool flag4)
    {
        if (flag1 || flag2 || flag3 is false || flag4)
        {
            return 42;
        }

        return -1;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3202_InvertNegativeIfWhenReturningOnAllPathsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3202_InvertNegativeIfWhenReturningOnAllPathsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3202_CodeFixProvider();
    }
}