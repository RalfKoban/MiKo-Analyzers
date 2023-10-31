using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6009_TryStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_try_finally_block_as_only_statement_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            try { } finally { }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_try_finally_block_as_only_statement_in_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    try { } finally { }

                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_try_finally_block_preceded_by_blank_line() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingElse();

            try { } finally { }
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_try_finally_block_followed_by_blank_line() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            try { } finally { }

            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_try_finally_block_not_preceded_by_blank_line() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingElse();
            try { } finally { }
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_try_finally_block_not_followed_by_blank_line() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            try { } finally { }
            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_try_finally_block_not_preceded_by_blank_line_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    DoSomethingElse();
                    try { } finally { }

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_try_finally_block_not_followed_by_blank_line_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    try { } finally { }
                    DoSomethingElse();

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_try_finally_block_not_preceded_by_blank_line()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomethingElse();
            try { } finally { }
        }

        private void DoSomethingElse()
        {
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
            DoSomethingElse();

            try { } finally { }
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_try_finally_block_not_followed_by_blank_line()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            try { } finally { }
            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
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
            try { } finally { }

            DoSomethingElse();
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_try_finally_block_not_followed_by_blank_line_in_switch_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    try { } finally { }
                    DoSomethingElse();

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    try { } finally { }

                    DoSomethingElse();

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_try_finally_block_not_preceded_by_blank_line_in_switch_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    DoSomethingElse();
                    try { } finally { }

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    DoSomethingElse();

                    try { } finally { }

                    break;
            }
        }

        private void DoSomethingElse()
        {
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6009_TryStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6009_TryStatementSurroundedByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6009_CodeFixProvider();
    }
}