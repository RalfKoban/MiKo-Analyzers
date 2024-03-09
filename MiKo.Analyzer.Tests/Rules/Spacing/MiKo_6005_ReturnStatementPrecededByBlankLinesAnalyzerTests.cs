using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6005_ReturnStatementPrecededByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_void_method_call() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_as_first_statement() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            return 1;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_yield_return_statement_as_first_statement() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(bool something)
        {
            yield return 1;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_yield_return_statements_in_a_row() => No_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething(bool something)
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_that_is_preceded_by_blank_line() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            var x = 0;

            return x;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_inside_if_block_without_brackets() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            var x = 0;
            if (something)
                return 1;

            return 2;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_inside_if_block_without_brackets_but_placed_on_same_line() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            var x = 0;
            if (something) return 1;

            return 2;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_inside_else_block_without_brackets_but_placed_on_same_line() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            var x = 0;
            if (something) return 1; else return 2;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_inside_if_block_with_brackets() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            if (something)
            {
                return 1;
            }

            return 2;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_inside_else_block_with_brackets() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            if (something)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_as_first_statement_inside_switch_case_block() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public bool DoSomething(int something)
        {
            switch (something)
            {
                case 0: return true;
                case 1: return true;
                case 2: return true;
                default: return false;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_as_first_statement_in_parenthesized_lambda_assignment() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            Callback(() =>
                         {
                             return 1 + DoSomething(something);
                         });

            return 2;
        }

        private static T Callback<T>(Func<T> a) => a();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_as_first_statement_in_parenthesized_lambda_assignment_if_variable_is_assigned_before_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            var x = 1;
            Callback(() =>
                         {
                             return x + DoSomething(something);
                         });

            return 2;
        }

        private static T Callback<T>(Func<T> a) => a();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_in_static_property_after_lock_with_blank_line_in_between() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public static TestMe Instance
        {
            get
            {
                lock(new object())
                {
                    _instance = new TestMe();
                }

                return _instance;
            }
        }

        private static TestMe _instance;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_in_property_after_lock_with_blank_line_in_between() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int SomeProperty
        {
            get
            {
                lock(this)
                {
                }

                return 42;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_return_statement_after_variable_assignment_inside_local_function_with_blank_line_in_between() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            return DoSomethingCore(something);

            int DoSomethingCore(bool something)
            {
                var x = 0;

                return x;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_void_statement_as_last_statement_in_if_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                DoSomething(false);
                return;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_after_if_block_without_separate_blank_line() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            return 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_after_variable_assignment_inside_if_block_without_separate_blank_line() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            if (something)
            {
                var x = 0;
                return x;
            }

            return 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_after_variable_assignment_inside_else_block_without_separate_blank_line() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            if (something)
            {
                return 1;
            }
            else
            {
                var x = 0;
                return x;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_after_variable_assignment_inside_parenthesized_lambda_assignment_without_separate_blank_line() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            Callback(() =>
                         {
                             var x = 1;
                             return x + DoSomething(something);
                         });

            return 2;
        }

        private static T Callback<T>(Func<T> a) => a();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_after_variable_assignment_inside_switch_case_block_without_blank_line_in_between() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {

        public bool DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    var x = true;
                    return x;

                default:
                    return false;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_after_variable_assignment_inside_switch_default_block_without_blank_line_in_between() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {

        public bool DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    return true;

                default:
                    var result = false;
                    return result;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_yield_return_statement_after_variable_assignment_without_blank_line_in_between() => An_issue_is_reported_for(@"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {

        public IEnumerable<int> DoSomething()
        {
            var x = 0;
            yield return x;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_static_return_statement_in_property_after_lock_without_blank_line_in_between() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public static int SomeProperty
        {
            get
            {
                lock(new object())
                {
                }
                return 42;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_in_property_after_lock_without_blank_line_in_between() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public static TestMe Instance
        {
            get
            {
                lock(new object())
                {
                    _instance = new TestMe();
                }
                return _instance;
            }
        }

        private static TestMe _instance;
    }
}
");

        [Test]
        public void An_issue_is_reported_for_return_statement_after_variable_assignment_inside_local_function_without_separate_blank_line() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            return DoSomethingCore(something);

            int DoSomethingCore(bool something)
            {
                var x = 0;
                return x;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_return_statement_and_missing_preceding_line()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            int x = 1;
            return x;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        public int DoSomething()
        {
            int x = 1;

            return x;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_yield_return_statement_and_missing_preceding_line()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething()
        {
            int x = 1;
            yield return x;
        }
    }
}
";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething()
        {
            int x = 1;

            yield return x;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_yield_return_statements_and_missing_preceding_line()
        {
            const string OriginalCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething()
        {
            int x = 1;
            yield return x;
            yield return x + 1;
            yield return x + 2;
        }
    }
}
";

            const string FixedCode = @"
using System.Collections.Generic;

namespace Bla
{
    public class TestMe
    {
        public IEnumerable<int> DoSomething()
        {
            int x = 1;

            yield return x;
            yield return x + 1;
            yield return x + 2;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6005_ReturnStatementPrecededByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6005_ReturnStatementPrecededByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6005_CodeFixProvider();
    }
}