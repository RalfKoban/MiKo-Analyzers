using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6014_WhileStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_while_block_as_only_statement_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            while (true)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_while_block_as_statement_with_blank_line_after_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;

            while (condition)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_while_block_as_statement_with_blank_line_before_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            while (true)
            {
            }

            var data = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_while_blocks_as_statements_with_blank_line_between_both_blocks_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            while (true)
            {
            }

            while (true)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_while_block_as_statement_without_blank_line_after_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;
            while (condition)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_while_block_as_statement_without_blank_line_before_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            while (true)
            {
            }
            var data = 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_while_blocks_as_statements_without_blank_line_between_both_blocks_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            while (true)
            {
            }
            while (true)
            {
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_while_block_as_statement_without_blank_line_after_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;
            while (condition)
            {
            }
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;

            while (condition)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_while_block_as_statement_without_blank_line_before_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            while (true)
            {
            }
            var data = 42;
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            while (true)
            {
            }

            var data = 42;
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_while_blocks_as_statements_without_blank_line_between_both_blocks_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            while (true)
            {
            }
            while (true)
            {
            }
        }
    }
}";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            while (true)
            {
            }

            while (true)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6014_WhileStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6014_WhileStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6014_CodeFixProvider();
    }
}