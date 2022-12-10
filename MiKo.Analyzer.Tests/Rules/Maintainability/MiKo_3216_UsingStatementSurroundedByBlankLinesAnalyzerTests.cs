using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public class MiKo_3216_UsingStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_using_block_as_only_statement_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            using (null)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_using_block_as_statement_with_blank_line_after_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;

            using (null)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_using_block_as_statement_with_blank_line_before_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            using (null)
            {
            }

            var data = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_using_blocks_as_statements_with_blank_line_between_both_blocks_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            using (null)
            {
            }

            using (null)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_using_blocks_as_statements_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            using (null)
            {
                using (null)
                {
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_using_block_as_statement_without_blank_line_after_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;
            using (null)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_using_block_as_statement_without_blank_line_before_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            using (null)
            {
            }
            var data = 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_using_blocks_as_statements_without_blank_line_between_both_blocks_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            using (null)
            {
            }
            using (null)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_using_block_as_statements_without_blank_line_after_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var someCondition = false;

            using (null)
            {
                var condition = true;
                using (null)
                {
                }
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_using_block_as_statement_without_blank_line_after_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;
            using (null)
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

            using (null)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_using_block_as_statement_without_blank_line_before_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            using (null)
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
            using (null)
            {
            }

            var data = 42;
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_using_blocks_as_statements_without_blank_line_between_both_blocks_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            using (null)
            {
            }
            using (null)
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
            using (null)
            {
            }

            using (null)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_using_block_as_statements_without_blank_line_after_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var someCondition = false;

            using (null)
            {
                var condition = true;
                using (null)
                {
                }
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var someCondition = false;

            using (null)
            {
                var condition = true;

                using (null)
                {
                }
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3216_UsingStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_3216_UsingStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3216_CodeFixProvider();
    }
}