using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6020_ThrowStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_throw_statement_as_statement_with_blank_line_after_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;

            throw new Exception();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_throw_statement_as_statement_with_blank_line_before_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool condition)
        {
            if (condition)
                throw new Exception();

            condition = true;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_throw_statement_in_block_as_statement_with_blank_line_before_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool condition)
        {
            if (condition)
            {
                throw new Exception();
            }

            condition = true;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_throw_statement_as_statement_with_no_blank_line_after_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;
            throw new Exception();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_throw_statement_in_if_statement_as_statement_with_no_blank_line_before_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool condition)
        {
            if (condition)
                throw new Exception();
            condition = false;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_throw_statement_in_single_line_if_statement_as_statement_with_no_blank_line_before_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool condition)
        {
            if (condition) throw new Exception();
            condition = false;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_throw_statement_as_statement_with_no_blank_line_after_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var condition = true;
            throw new Exception();
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

            throw new Exception();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_throw_statement_in_if_statement_as_statement_with_no_blank_line_before_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool condition)
        {
            if (condition)
                throw new Exception();
            condition = false;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool condition)
        {
            if (condition)
                throw new Exception();

            condition = false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_throw_statement_in_single_line_if_statement_as_statement_with_no_blank_line_before_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool condition)
        {
            if (condition) throw new Exception();
            condition = false;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool condition)
        {
            if (condition) throw new Exception();

            condition = false;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6020_ThrowStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6020_ThrowStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6020_CodeFixProvider();
    }
}