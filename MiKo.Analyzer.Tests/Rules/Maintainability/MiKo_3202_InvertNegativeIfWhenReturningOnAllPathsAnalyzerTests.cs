using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
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

        // TODO RKN: Test conditionals, if/else, follow up code
        protected override string GetDiagnosticId() => MiKo_3202_InvertNegativeIfWhenReturningOnAllPathsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3202_InvertNegativeIfWhenReturningOnAllPathsAnalyzer();

        // protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3202_CodeFixProvider();
    }
}