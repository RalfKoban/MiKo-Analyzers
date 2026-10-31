using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6078_ForEachVariableStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_foreach_variable_block_as_only_statement_in_method_with_var_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach (var (a, b) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_statement_with_blank_line_after_variable_assignment_in_method_with_var_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;

            foreach (var (a, b) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_statement_with_blank_line_before_variable_assignment_in_method_with_var_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach (var (a, b) in values)
            {
            }

            var data = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_blocks_as_statements_with_blank_line_between_both_blocks_in_method_with_var_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach (var (a, b) in values)
            {
            }

            foreach (var (c, d) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_variable_block_as_statement_without_blank_line_after_variable_assignment_in_method_with_var_tuple() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;
            foreach (var (a, b) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_variable_block_as_statement_without_blank_line_before_variable_assignment_in_method_with_var_tuple() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach (var (a, b) in values)
            {
            }
            var data = 42;
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_foreach_variable_blocks_as_statements_without_blank_line_between_both_blocks_in_method_with_var_tuple() => An_issue_is_reported_for(2, @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach (var (a, b) in values)
            {
            }
            foreach (var (c, d) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_foreach_variable_block_as_statement_without_blank_line_after_variable_assignment_in_method_with_var_tuple()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;
            foreach (var (a, b) in values)
            {
            }
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;

            foreach (var (a, b) in values)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_foreach_variable_block_as_statement_without_blank_line_before_variable_assignment_in_method_with_var_tuple()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach (var (a, b) in values)
            {
            }
            var data = 42;
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach (var (a, b) in values)
            {
            }

            var data = 42;
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_foreach_variable_blocks_as_statements_without_blank_line_between_both_blocks_in_method_with_var_tuple()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach (var (a, b) in values)
            {
            }
            foreach (var (c, d) in values)
            {
            }
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach (var (a, b) in values)
            {
            }

            foreach (var (c, d) in values)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_only_statement_in_method_with_typed_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((int a, int b) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_statement_with_blank_line_after_variable_assignment_in_method_with_typed_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;

            foreach ((int a, int b) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_statement_with_blank_line_before_variable_assignment_in_method_with_typed_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((int a, int b) in values)
            {
            }

            var data = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_blocks_as_statements_with_blank_line_between_both_blocks_in_method_with_typed_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((int a, int b) in values)
            {
            }

            foreach ((int c, int d) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_variable_block_as_statement_without_blank_line_after_variable_assignment_in_method_with_typed_tuple() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;
            foreach ((int a, int b) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_variable_block_as_statement_without_blank_line_before_variable_assignment_in_method_with_typed_tuple() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((int a, int b) in values)
            {
            }
            var data = 42;
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_foreach_variable_blocks_as_statements_without_blank_line_between_both_blocks_in_method_with_typed_tuple() => An_issue_is_reported_for(2, @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((int a, int b) in values)
            {
            }
            foreach ((int c, int d) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_foreach_variable_block_as_statement_without_blank_line_after_variable_assignment_in_method_with_typed_tuple()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;
            foreach ((int a, int b) in values)
            {
            }
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;

            foreach ((int a, int b) in values)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_foreach_variable_block_as_statement_without_blank_line_before_variable_assignment_in_method_with_typed_tuple()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((int a, int b) in values)
            {
            }
            var data = 42;
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((int a, int b) in values)
            {
            }

            var data = 42;
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_foreach_variable_blocks_as_statements_without_blank_line_between_both_blocks_in_method_with_typed_tuple()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((int a, int b) in values)
            {
            }
            foreach ((int c, int d) in values)
            {
            }
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((int a, int b) in values)
            {
            }

            foreach ((int c, int d) in values)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_only_statement_in_method_with_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((a, b) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_statement_with_blank_line_after_variable_assignment_in_method_with_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;

            foreach ((a, b) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_statement_with_blank_line_before_variable_assignment_in_method_with_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((a, b) in values)
            {
            }

            var data = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_blocks_as_statements_with_blank_line_between_both_blocks_in_method_with_tuple() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((a, b) in values)
            {
            }

            foreach ((c, d) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_variable_block_as_statement_without_blank_line_after_variable_assignment_in_method_with_tuple() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;
            foreach ((a, b) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_variable_block_as_statement_without_blank_line_before_variable_assignment_in_method_with_tuple() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((a, b) in values)
            {
            }
            var data = 42;
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_foreach_variable_blocks_as_statements_without_blank_line_between_both_blocks_in_method_with_tuple() => An_issue_is_reported_for(2, @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((a, b) in values)
            {
            }
            foreach ((c, d) in values)
            {
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_foreach_variable_block_as_statement_without_blank_line_after_variable_assignment_in_method_with_tuple()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;
            foreach ((a, b) in values)
            {
            }
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            var data = 42;

            foreach ((a, b) in values)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_foreach_variable_block_as_statement_without_blank_line_before_variable_assignment_in_method_with_tuple()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((a, b) in values)
            {
            }
            var data = 42;
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((a, b) in values)
            {
            }

            var data = 42;
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_foreach_variable_blocks_as_statements_without_blank_line_between_both_blocks_in_method_with_tuple()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((a, b) in values)
            {
            }
            foreach ((c, d) in values)
            {
            }
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<(int, int)> values)
        {
            foreach ((a, b) in values)
            {
            }

            foreach ((c, d) in values)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_only_statement_in_method_with_discard() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (_ in values)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_statement_with_blank_line_after_variable_assignment_in_method_with_discard() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            var data = 42;

            foreach (_ in values)
            {
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_block_as_statement_with_blank_line_before_variable_assignment_in_method_with_discard() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (_ in values)
            {
            }

            var data = 42;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_foreach_variable_blocks_as_statements_with_blank_line_between_both_blocks_in_method_with_discard() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (_ in values)
            {
            }

            foreach (_ in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_variable_block_as_statement_without_blank_line_after_variable_assignment_in_method_with_discard() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            var data = 42;
            foreach (_ in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_foreach_variable_block_as_statement_without_blank_line_before_variable_assignment_in_method_with_discard() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (_ in values)
            {
            }
            var data = 42;
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_foreach_variable_blocks_as_statements_without_blank_line_between_both_blocks_in_method_with_discard() => An_issue_is_reported_for(2, @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (_ in values)
            {
            }
            foreach (_ in values)
            {
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_foreach_variable_block_as_statement_without_blank_line_after_variable_assignment_in_method_with_discard()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            var data = 42;
            foreach (_ in values)
            {
            }
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            var data = 42;

            foreach (_ in values)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_foreach_variable_block_as_statement_without_blank_line_before_variable_assignment_in_method_with_discard()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (_ in values)
            {
            }
            var data = 42;
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (_ in values)
            {
            }

            var data = 42;
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_foreach_variable_blocks_as_statements_without_blank_line_between_both_blocks_in_method_with_discard()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (_ in values)
            {
            }
            foreach (_ in values)
            {
            }
        }
    }
}";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(IEnumerable<int> values)
        {
            foreach (_ in values)
            {
            }

            foreach (_ in values)
            {
            }
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6078_ForEachVariableStatementSurroundedByBlankLinesAnalyzer();

        protected override string GetDiagnosticId() => MiKo_6078_ForEachVariableStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6078_CodeFixProvider();
    }
}