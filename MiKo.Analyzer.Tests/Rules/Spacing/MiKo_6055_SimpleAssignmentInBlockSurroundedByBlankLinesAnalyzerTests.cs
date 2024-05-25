using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6055_SimpleAssignmentInBlockSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
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
        public void An_issue_is_reported_for_assignment_followed_by_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething()
        {
            m_field = 1;
            {
                // some comment
            }
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
        public void An_issue_is_reported_for_assignment_followed_by_if_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_switch_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            switch (something)
            {
                default: break;
            }
            m_field = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_followed_by_switch_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            switch (something)
            {
                default: break;
            }
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
        public void An_issue_is_reported_for_assignment_followed_by_method_call() => An_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_assignment_followed_by_method_call_inside_switch_section() => An_issue_is_reported_for(@"
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
                    m_field = 1;
                    DoSomething(42);

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_while_loop() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            while (something)
            {
            }
            m_field = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_followed_by_while_loop() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            while (something)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_do_while_loop() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            do
            {
            } while (something);
            m_field = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_followed_by_do_while_loop() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            do
            {
            } while (something);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_for_loop() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            for (int i = 0; i < 10; i++)
            {
            }
            m_field = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_followed_by_for_loop() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            for (int i = 0; i < 10; i++)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_foreach_loop() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(System.Collections.Generic.IEnumerable<int> values)
        {
            foreach (var value in values)
            {
            }
            m_field = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_followed_by_foreach_loop() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(System.Collections.Generic.IEnumerable<int> values)
        {
            m_field = 1;
            foreach (var value in values)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_lock_statement() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            lock (this)
            {
            }
            m_field = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_followed_by_lock_statement() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            lock (this)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_preceded_by_using_statement() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe : System.IDisposable
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            using (this)
            {
            }
            m_field = 1;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_assignment_followed_by_using_statement() => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe : System.IDisposable
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            using (this)
            {
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
        public void Code_gets_fixed_for_assignment_followed_by_block()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething()
        {
            m_field = 1;
            {
                // some comment
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

        public void DoSomething()
        {
            m_field = 1;

            {
                // some comment
            }
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
        public void Code_gets_fixed_for_assignment_followed_by_if_block()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            if (something)
            {
                // some comment
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

        public void DoSomething(bool something)
        {
            m_field = 1;

            if (something)
            {
                // some comment
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_switch_block()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            switch (something)
            {
                default: break;
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
            switch (something)
            {
                default: break;
            }

            m_field = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_followed_by_switch_block()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            switch (something)
            {
                default: break;
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

        public void DoSomething(bool something)
        {
            m_field = 1;

            switch (something)
            {
                default: break;
            }
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
        public void Code_gets_fixed_for_assignment_followed_by_method_call()
        {
            const string OriginalCode = @"
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
";

            const string FixedCode = @"
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

        [Test]
        public void Code_gets_fixed_for_assignment_followed_by_method_call_inside_switch_section()
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
                    m_field = 1;
                    DoSomething(42);

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
                    m_field = 1;

                    DoSomething(42);

                    break;
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_while_loop()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            while (something)
            {
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
            while (something)
            {
            }

            m_field = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_followed_by_while_loop()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            while (something)
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
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;

            while (something)
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_do_while_loop()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            do
            {
            } while (something);
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
            do
            {
            } while (something);

            m_field = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_followed_by_do_while_loop()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            do
            {
            } while (something);
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
            m_field = 1;

            do
            {
            } while (something);
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_for_loop()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            for (int i = 0; i < 10; i++)
            {
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
            for (int i = 0; i < 10; i++)
            {
            }

            m_field = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_followed_by_for_loop()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            for (int i = 0; i < 10; i++)
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
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;

            for (int i = 0; i < 10; i++)
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_foreach_loop()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(System.Collections.Generic.IEnumerable<int> values)
        {
            foreach (var value in values)
            {
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

        public void DoSomething(System.Collections.Generic.IEnumerable<int> values)
        {
            foreach (var value in values)
            {
            }

            m_field = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_followed_by_foreach_loop()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(System.Collections.Generic.IEnumerable<int> values)
        {
            m_field = 1;
            foreach (var value in values)
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
        private int m_field;

        public void DoSomething(System.Collections.Generic.IEnumerable<int> values)
        {
            m_field = 1;

            foreach (var value in values)
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_lock_statement()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            lock (this)
            {
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
            lock (this)
            {
            }

            m_field = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_followed_by_lock_statement()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            lock (this)
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
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;

            lock (this)
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_preceded_by_using_statement()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe : System.IDisposable
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            using (this)
            {
            }
            m_field = 1;
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe : System.IDisposable
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            using (this)
            {
            }

            m_field = 1;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_assignment_followed_by_using_statement()
        {
            const string OriginalCode = @"
namespace Bla
{
    public class TestMe : System.IDisposable
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;
            using (this)
            {
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public class TestMe : System.IDisposable
    {
        private int m_field;

        public void DoSomething(bool something)
        {
            m_field = 1;

            using (this)
            {
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6055_SimpleAssignmentInBlockSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6055_SimpleAssignmentInBlockSurroundedByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6055_CodeFixProvider();
    }
}