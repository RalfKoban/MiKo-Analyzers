using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public class MiKo_6019_ContinueStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_continue_statement_as_statement_with_blank_line_after_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;

            while (condition)
            {
                condition = false;

                continue;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_continue_statement_as_statement_with_blank_line_before_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;

            while (condition)
            {
                continue;

                condition = false;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_continue_statement_as_statement_without_blank_line_after_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;

            while (condition)
            {
                condition = false;
                continue;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_continue_statement_as_statement_without_blank_line_before_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;

            while (condition)
            {
                continue;
                condition = false;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_continue_statement_as_statement_without_blank_line_after_variable_assignment_in_method()
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
                condition = false;
                continue;
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
                condition = false;

                continue;
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_continue_statement_as_statement_without_blank_line_before_variable_assignment_in_method()
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
                continue;
                condition = false;
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
                continue;

                condition = false;
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_continue_statement_as_statement_without_blank_line_after_variable_assignment_in_switch_case_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(GCCollectionMode mode)
        {
            int value = 0;

            switch (mode)
            {
                case GCCollectionMode.Default:
                    value = 1;
                    continue;
                case GCCollectionMode.Forced:
                    value = 2;
                    continue;
                case GCCollectionMode.Optimized:
                    value = 3;
                    continue;
                default:
                    value = 4;
                    continue;
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
        public void DoSomething(GCCollectionMode mode)
        {
            int value = 0;

            switch (mode)
            {
                case GCCollectionMode.Default:
                    value = 1;

                    continue;

                case GCCollectionMode.Forced:
                    value = 2;

                    continue;

                case GCCollectionMode.Optimized:
                    value = 3;

                    continue;

                default:
                    value = 4;

                    continue;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6019_ContinueStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6019_ContinueStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6019_CodeFixProvider();
    }
}