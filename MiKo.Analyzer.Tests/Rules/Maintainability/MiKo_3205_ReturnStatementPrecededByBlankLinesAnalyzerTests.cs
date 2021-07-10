using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3205_ReturnStatementPrecededByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_void_method_call() => No_issue_is_reported_for(@"
using NUnit.Framework;

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
using NUnit.Framework;

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
        public void No_issue_is_reported_for_return_statement_that_is_preceded_by_blank_line() => No_issue_is_reported_for(@"
using NUnit.Framework;

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
using NUnit.Framework;

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
using NUnit.Framework;

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
using NUnit.Framework;

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
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            var x = 0;
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
        public void No_issue_is_reported_for_return_statement_as_first_statement_inside_switch_case_block() => No_issue_is_reported_for(@"
using NUnit.Framework;

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
using NUnit.Framework;

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
using NUnit.Framework;

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
        public void An_issue_is_reported_for_return_statement_preceded_by_if_block_without_separate_blank_line() => An_issue_is_reported_for(@"
using NUnit.Framework;

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
        public void An_issue_is_reported_for_return_statement_preceded_by_variable_assignment_inside_if_block_without_separate_blank_line() => An_issue_is_reported_for(@"
using NUnit.Framework;

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
        public void An_issue_is_reported_for_return_statement_preceded_by_variable_assignment_inside_parenthesized_lambda_assignment_without_separate_blank_line() => An_issue_is_reported_for(@"
using NUnit.Framework;

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
using NUnit.Framework;

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
using NUnit.Framework;

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
        public void Code_gets_fixed_for_missing_preceding_line()
        {
            const string OriginalCode = @"
using NUnit.Framework;

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
using NUnit.Framework;

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

        protected override string GetDiagnosticId() => MiKo_3205_ReturnStatementPrecededByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3205_ReturnStatementPrecededByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3205_CodeFixProvider();
    }
}