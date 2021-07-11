using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_Log_call_followed_by_another_Log_call() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        void Debug();
        void Info();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            Log.Debug();
            Log.Info();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Log_call_inside_if_statement_followed_by_block_statement() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Log_call_followed_by_if_block_separated_by_blank_line() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            Log.Debug();

            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Log_call_preceded_by_if_block_separated_by_blank_line() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Log.Debug();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Log_call_inside_else_block() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            else
                Log.Debug();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Log_call_followed_by_blank_line_in_switch_section() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    Log.Debug();

                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Log_call_followed_by_if_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            Log.Debug();
            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Log_call_followed_by_code_in_switch_section() => An_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int something)
        {
            switch (something)
            {
                case 0:
                    Log.Debug();
                    break;
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Log_call_preceded_by_if_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Log.Debug();
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line()
        {
            const string OriginalCode = @"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Log.Debug();
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Log.Debug();
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
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            Log.Debug();
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
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            Log.Debug();

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
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Log.Debug();
            Log.Debug();
            Log.Debug();
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
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Log.Debug();
            Log.Debug();
            Log.Debug();

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
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Log.Debug();
            var x = 12;
            Log.Debug();
            var y = x;
            Log.Debug();
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
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Log.Debug();

            var x = 12;

            Log.Debug();

            var y = x;

            Log.Debug();

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
        public void Code_gets_fixed_for_missing_preceding_line_for_if_DebugEnabled_block()
        {
            const string OriginalCode = @"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            something = false;
            if (Log.IsDebugEnabled)
            {
                Log.Debug();
            }
        }
    }
}
";

            const string FixedCode = @"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            something = false;

            if (Log.IsDebugEnabled)
            {
                Log.Debug();
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3201_CodeFixProvider();
    }
}