using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6070_ConsoleStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_Console_call_followed_by_another_Console_call() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Console_call_directly_behind_if_when_if_is_not_separated_by_blank_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            something = true;
            if (something) Console.WriteLine();
            something = false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Console_call_followed_by_if_block_separated_by_blank_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            Console.WriteLine();

            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Console_call_preceded_by_if_block_separated_by_blank_line() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Console.WriteLine();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Console_call_inside_else_block() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            else
                Console.WriteLine();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Console_call_followed_by_blank_line_in_switch_section() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    Console.WriteLine();

                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Console_call_in_Moq_call() => No_issue_is_reported_for(@"
using Moq;

using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var mock = new Mock<Console>();

            mock.Verify(_ => _.WriteLine()), Times.Once);
            mock.Verify(_ => _.ReadLine()), Times.Never);
            mock.Verify(_ => _.WriteLine(""some text"")), Times.Once);
            mock.Verify(_ => _.Write(""some text"")), Times.Never);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Console_call_followed_by_if_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            Console.WriteLine();
            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Console_call_followed_by_code_in_switch_section() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    Console.WriteLine();
                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Console_call_preceded_by_if_block() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Console.WriteLine();
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Console.WriteLine();
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Console.WriteLine();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_missing_following_line()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            Console.WriteLine();
            if (something)
            {
                // some comment
            }
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            Console.WriteLine();

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
        public void Code_gets_fixed_for_missing_preceding_and_following_line_for_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            if (something)
            {
                // some comment
            }
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

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
        public void Code_gets_fixed_for_multiple_missing_preceding_and_following_line_for_block()
        {
            const string OriginalCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Console.WriteLine();
            var x = 12;
            Console.WriteLine();
            var y = x;
            Console.WriteLine();
            if (something)
            {
                // some comment
            }
        }
    }
}
";

            const string FixedCode = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Console.WriteLine();

            var x = 12;

            Console.WriteLine();

            var y = x;

            Console.WriteLine();

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

        protected override string GetDiagnosticId() => MiKo_6070_ConsoleStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6070_ConsoleStatementSurroundedByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6070_CodeFixProvider();
    }
}