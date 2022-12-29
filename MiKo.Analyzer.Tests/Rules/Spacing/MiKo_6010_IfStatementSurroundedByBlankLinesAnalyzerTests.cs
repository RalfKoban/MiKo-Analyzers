﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public class MiKo_6010_IfStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_if_block_as_only_statement_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            if (true)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_if_else_if_block_as_only_statement_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            if (true)
            {
            }
            else if (false)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_if_block_as_statement_with_blank_line_after_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;

            if (condition)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_second_if_block_as_statement_with_blank_line_after_first_if_block_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            if (true)
            {
            }

            if (false)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_nested_if_block_as_statement_inside_if_block_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            if (true)
            {
                if (false)
                {
                }
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_if_block_as_statement_with_blank_line_after_variable_assignment_in_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var condition = true;

                    if (condition)
                    {
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_if_block_as_statement_after_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;
            if (condition)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_second_if_block_as_statement_after_first_if_block_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            if (true)
            {
            }
            if (false)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_if_block_as_statement_after_variable_assignment_if_block_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            if (true)
            {
                var condition = false;
                if (condition)
                {
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_if_block_as_statement_with_blank_line_after_variable_assignment_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var condition = true;
                    if (condition)
                    {
                    }

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_if_block_as_statement_with_blank_line_after_block_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int i)
        {
            switch (i)
            {
                case 1:
                    var condition = true;

                    if (condition)
                    {
                    }
                    break;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_if_block_as_statement_after_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;
            if (condition)
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
            var condition = true;

            if (condition)
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_second_if_block_as_statement_after_first_if_block_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            if (true)
            {
            }
            if (false)
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
            if (true)
            {
            }

            if (false)
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nested_if_block_as_statement_after_variable_assignment_if_block_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            if (true)
            {
                var condition = false;
                if (condition)
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
            if (true)
            {
                var condition = false;

                if (condition)
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
        public void Code_gets_fixed_for_if_block_as_statement_with_blank_line_after_variable_assignment_in_switch_section()
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
                    var condition = true;
                    if (condition)
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
                    var condition = true;

                    if (condition)
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
        public void Code_gets_fixed_for_if_block_as_statement_with_blank_line_after_block_in_switch_section()
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
                    var condition = true;

                    if (condition)
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
                    var condition = true;

                    if (condition)
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

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6010_IfStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6010_IfStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6010_CodeFixProvider();
    }
}