using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6012_ForEachStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_foreach_block_as_only_statement_in_method() => No_issue_is_reported_for(@"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_block_as_statement_with_blank_line_after_variable_assignment_in_method() => No_issue_is_reported_for(@"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var values = Enumerable.Empty<int>();

            foreach (var value in values)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_block_as_statement_with_blank_line_before_variable_assignment_in_method() => No_issue_is_reported_for(@"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
            }

            var data = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_blocks_as_statements_with_blank_line_between_both_blocks_in_method() => No_issue_is_reported_for(@"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
            }

            foreach (var value in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_block_as_statement_without_blank_line_after_variable_assignment_in_method() => An_issue_is_reported_for(@"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var values = Enumerable.Empty<int>();
            foreach (var value in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_block_as_statement_without_blank_line_before_variable_assignment_in_method() => An_issue_is_reported_for(@"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
            }
            var data = 42;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_blocks_as_statements_without_blank_line_between_both_blocks_in_method() => An_issue_is_reported_for(@"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
            }
            foreach (var value in values)
            {
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_foreach_block_as_statement_without_blank_line_after_variable_assignment_in_method()
        {
            const string OriginalCode = @"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var values = Enumerable.Empty<int>();
            foreach (var value in values)
            {
            }
        }
    }
}";

            const string FixedCode = @"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var values = Enumerable.Empty<int>();

            foreach (var value in values)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_foreach_block_as_statement_without_blank_line_before_variable_assignment_in_method()
        {
            const string OriginalCode = @"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            foreach (var value in values)
            {
            }
            var data = 42;
        }
    }
}";

            const string FixedCode = @"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            foreach (var value in values)
            {
            }

            var data = 42;
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_foreach_blocks_as_statements_without_blank_line_between_both_blocks_in_method()
        {
            const string OriginalCode = @"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
            }
            foreach (var value in values)
            {
            }
        }
    }
}";

            const string FixedCode = @"
using System.Generic.Collections;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (var value in values)
            {
            }

            foreach (var value in values)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6012_ForEachStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6012_ForEachStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6012_CodeFixProvider();
    }
}