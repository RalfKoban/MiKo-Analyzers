using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3204_InvertNegativeIfWhenFollowedByElseBlockAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_method_with_positive_if_statement_that_has_an_else_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
            return 42;
        else
            return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_positive_if_statement_that_does_not_return_and_has_no_else_block() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_method_with_negative_if_statement_that_returns_and_has_no_else_block() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            return 42;
        }

        return 42;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_is_false_if_statement_that_does_not_return_and_has_no_else_block() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_method_with_is_false_if_statement_that_returns_and_has_no_else_block_but_follow_up_code() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_method_with_negative_else_if_statement_that_returns_and_else_block_that_returns() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return 0815;
        }
        else if (!flag)
        {
            return 42;
        }
        else
        {
            return -1;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_is_false_else_if_statement_that_returns_and_else_block_that_returns() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag)
        {
            return 0815;
        }
        else if (flag is false)
        {
            return 42;
        }
        else
        {
            return -1;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_positive_and_negative_if_statement_that_returns_and_an_else_block_with_code() => No_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag1, bool flag2)
    {
        if (flag1 || !flag2)
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
        public void An_issue_is_reported_for_method_with_negative_if_statement_that_returns_and_an_else_block_with_code() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_method_with_is_false_if_statement_that_returns_and_an_else_block_with_code() => An_issue_is_reported_for(@"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (flag is false)
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
        public void Code_gets_fixed_for_method_with_negative_if_block_and_else_block()
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
        public void Code_gets_fixed_for_method_with_negative_if_block_and_else_statement()
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
        {
            return -1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_negative_if_statement_and_else_block()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
            return -1;
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
            return -1;
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_negative_if_statement_and_else_statement()
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
        public void Code_gets_fixed_for_method_with_is_false_if_block_and_else_block()
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
        public void Code_gets_fixed_for_method_with_negative_if_block_and_else_block_with_comments_within_blocks()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag)
        {
            // some if comment
            return -1;
        }
        else
        {
            // some else comment
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
            // some else comment
            return 42;
        }
        else
        {
            // some if comment
            return -1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_negative_if_block_and_else_block_with_comments_after_clauses()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        if (!flag) // some if comment
        {
            return -1;
        }
        else // some else comment
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
        if (flag) // some else comment
        {
            return 42;
        }
        else // some if comment
        {
            return -1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_with_negative_if_block_and_else_block_with_comment_before_if()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        // some comment
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
        // some comment
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
        public void Code_gets_fixed_for_method_with_negative_if_block_and_else_block_with_comment_before_if_and_else()
        {
            const string OriginalCode = @"
public class TestMe
{
    public int DoSomething(bool flag)
    {
        // some if comment
        if (!flag)
        {
            return -1;
        }
        // some else comment
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
        // some else comment
        if (flag)
        {
            return 42;
        }
        // some if comment
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
        public void Code_gets_fixed_for_method_with_negative_if_block_and_else_block_with_comment_before_else_only()
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
        // some comment
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
        // some comment
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

        protected override string GetDiagnosticId() => MiKo_3204_InvertNegativeIfWhenFollowedByElseBlockAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3204_InvertNegativeIfWhenFollowedByElseBlockAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3204_CodeFixProvider();
    }
}