using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6013_ForStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_for_block_as_only_statement_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            for (var i = 0; i < 10; i++)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_for_block_as_statement_with_blank_line_after_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var count = 42;

            for (var i = 0; i < count; i++)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_for_block_as_statement_with_blank_line_before_variable_assignment_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            for (var i = 0; i < 10; i++)
            {
            }

            var data = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_for_blocks_as_statements_with_blank_line_between_both_blocks_in_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            for (var i = 0; i < 10; i++)
            {
            }

            for (var i = 0; i < 10; i++)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_for_block_as_statement_without_blank_line_after_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var count = 42;
            for (var i = 0; i < count; i++)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_for_block_as_statement_without_blank_line_before_variable_assignment_in_method() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            for (var i = 0; i < 10; i++)
            {
            }
            var data = 42;
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Would look strange otherwise.")]
        [Test]
        public void An_issue_is_reported_for_for_blocks_as_statements_without_blank_line_between_both_blocks_in_method() => An_issue_is_reported_for(2, @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            for (var i = 0; i < 10; i++)
            {
            }
            for (var i = 0; i < 10; i++)
            {
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_for_block_as_statement_without_blank_line_after_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var count = 42;
            for (var i = 0; i < count; i++)
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
            var count = 42;

            for (var i = 0; i < count; i++)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_for_block_as_statement_without_blank_line_before_variable_assignment_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            for (var i = 0; i < 10; i++)
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
            for (var i = 0; i < 10; i++)
            {
            }

            var data = 42;
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_for_blocks_as_statements_without_blank_line_between_both_blocks_in_method()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            for (var i = 0; i < 10; i++)
            {
            }
            for (var i = 0; i < 10; i++)
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
            for (var i = 0; i < 10; i++)
            {
            }

            for (var i = 0; i < 10; i++)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6013_ForStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6013_ForStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6013_CodeFixProvider();
    }
}