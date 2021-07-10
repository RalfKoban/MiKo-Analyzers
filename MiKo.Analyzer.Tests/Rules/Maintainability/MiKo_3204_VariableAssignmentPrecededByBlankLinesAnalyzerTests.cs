using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3204_VariableAssignmentPrecededByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_call() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_variable_assignment_as_first_statement() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 1;

            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_assignment_as_first_statement_after_variable_declaration() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            int x;
            x = 1;

            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_assignment_as_first_statement_after_variable_declaration_and_assignment() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            int x;

            var y = something ? 0 : -1;
            x = DoSomething(something) + y;

            return x;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_variable_assignment_as_first_statement_in_parenthesized_lambda_assignment() => No_issue_is_reported_for(@"
using System;
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public int DoSomething(bool something)
        {
            var y = something ? 0 : -1;
            int x;

            Callback(() => x = DoSomething(something) + y);
            Callback(() => y = DoSomething(something) + x);

            return x;
        }

        private void Callback(Action a) => a();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_variable_assignments_as_first_statements_after_variable_declarations() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            int x, y;
            x = 1;
            y = 2;

            DoSomething();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_assignment_preceded_by_if_block() => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x;

            if (something)
            {
                // some comment
            }
            x = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_variable_assignment_preceded_by_method_call() => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            int x;

            DoSomething();
            x = 1;
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line_after_if_block()
        {
            const string OriginalCode = @"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x;
            if (something)
            {
                // some comment
            }
            x = 1;
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
        public void DoSomething(bool something)
        {
            int x;
            if (something)
            {
                // some comment
            }

            x = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line_after_method_call()
        {
            const string OriginalCode = @"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x;
            DoSomething(something);
            x = 1;
            DoSomething(something);
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
        public void DoSomething(bool something)
        {
            int x;
            DoSomething(something);

            x = 1;
            DoSomething(something);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_multiple_variables_with_missing_preceding_line()
        {
            const string OriginalCode = @"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            int x, y;

            DoSomething(something);
            x = something;
            y = something;
            DoSomething(something);
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
        public void DoSomething(bool something)
        {
            int x, y;

            DoSomething(something);

            x = something;
            y = something;
            DoSomething(something);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line()
        {
            const string OriginalCode = @"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            int x;
            DoSomething();
            x = 1;
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
        public void DoSomething()
        {
            int x;
            DoSomething();

            x = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3204_VariableAssignmentPrecededByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3204_VariableAssignmentPrecededByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3204_CodeFixProvider();
    }
}