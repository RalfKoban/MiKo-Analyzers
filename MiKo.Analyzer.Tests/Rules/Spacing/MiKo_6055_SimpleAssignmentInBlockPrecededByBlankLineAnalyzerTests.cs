using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6055_SimpleAssignmentInBlockPrecededByBlankLineAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_call() => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_assignment_as_first_statement() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething()
        {
            m_field = 1;

            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_assignment_as_first_statement_in_parenthesized_lambda_assignment() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public int DoSomething(bool something)
        {
            Callback(() => m_field = DoSomething(something) + 1);
            Callback(() => m_field = DoSomething(something) + 2);

            return x;
        }

        private void Callback(Action a) => a();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_assignments_as_first_statements() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field1;
        private int m_field2;

        public void DoSomething()
        {
            m_field1 = 1;
            m_field2 = 2;

            DoSomething();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_assignments_as_first_statements_inside_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field1;
        private int m_field2;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    m_field1 = 1;
                    m_field2 = 2;

                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_multiple_assignments_as_first_statements_inside_block_inside_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field1;
        private int m_field2;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                {
                    m_field1 = 1;
                    m_field2 = 2;

                    break;
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething()
        {
            {
                // some comment
            }
            m_field = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_if_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            m_field = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_method_call() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething()
        {
            DoSomething();
            m_field = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_method_call_inside_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    DoSomething(42);
                    m_field = 1;

                    break;
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_block()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething()
        {
            {
                // some comment
            }
            m_field = 1;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething()
        {
            {
                // some comment
            }

            m_field = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_if_block()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            m_field = 1;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            m_field = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_method_call()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething()
        {
            DoSomething();
            m_field = 1;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething()
        {
            DoSomething();

            m_field = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_method_call_inside_switch_section()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    DoSomething(42);
                    m_field = 1;

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
        private int m_field;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    DoSomething(42);

                    m_field = 1;

                    break;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6055_SimpleAssignmentInBlockPrecededByBlankLineAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6055_SimpleAssignmentInBlockPrecededByBlankLineAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6055_CodeFixProvider();
    }
}