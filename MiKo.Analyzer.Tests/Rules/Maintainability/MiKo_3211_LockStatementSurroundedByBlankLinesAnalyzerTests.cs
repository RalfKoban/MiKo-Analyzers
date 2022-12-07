using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public class MiKo_3211_LockStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_lock_block_as_only_statement_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            lock (this)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_lock_block_as_statement_with_blank_line_after_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var sync = new object();

            lock (sync)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_second_lock_block_as_statement_with_blank_line_after_first_lock_block_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var sync = new object();

            lock (sync)
            {
            }

            lock (sync)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_lock_block_as_statement_inside_lock_block_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var sync = new object();

            lock (sync)
            {
                lock (sync)
                {
                }
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_lock_block_as_statement_in_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var sync = new object();

                    lock (sync)
                    {
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_lock_block_as_statement_after_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var sync = new object();
            lock (sync)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_second_lock_block_as_statement_after_first_lock_block_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var sync = new object();
            lock (sync)
            {
            }
            lock (sync)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_lock_block_as_statement_after_variable_assignment_lock_block_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            lock (this)
            {
                var sync = new object();
                lock (sync)
                {
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_lock_block_as_statement_after_variable_assignment_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var sync = new object();
                    lock (sync)
                    {
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_lock_block_as_statement_without_blank_line_after_block_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var sync = new object();

                    lock (sync)
                    {
                    }
                    break;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_lock_block_as_statement_after_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var sync = new object();
            lock (sync)
            {
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
            var sync = new object();

            lock (sync)
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_second_lock_block_as_statement_after_first_lock_block_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var sync = new object();

            lock (sync)
            {
            }
            lock (sync)
            {
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
            var sync = new object();

            lock (sync)
            {
            }

            lock (sync)
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_lock_block_as_statement_after_variable_assignment_side_lock_block_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            lock (this)
            {
                var sync = new object();
                lock (sync)
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
            lock (this)
            {
                var sync = new object();

                lock (sync)
                {
                }
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_lock_block_as_statement_after_variable_assignment_in_switch_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var sync = new object();
                    lock (sync)
                    {
                    }

                    break;
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
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var sync = new object();

                    lock (sync)
                    {
                    }

                    break;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_lock_block_as_statement_without_blank_line_after_block_in_switch_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var sync = new object();

                    lock (sync)
                    {
                    }
                    break;
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
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var sync = new object();

                    lock (sync)
                    {
                    }

                    break;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3211_LockStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_3211_LockStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3211_CodeFixProvider();
    }
}