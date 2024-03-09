using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [TestFixture]
    public sealed class MiKo_6001_LogStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_Log_call_followed_by_another_Log_call() => No_issue_is_reported_for(@"
namespace log4net
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
        public void No_issue_is_reported_for_Log_call_directly_behind_if_when_if_is_not_separated_by_blank_line() => No_issue_is_reported_for(@"
namespace log4net
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
            something = true;
            if (something) Log.Debug();
            something = false;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Log_call_inside_if_statement_followed_by_block_statement() => No_issue_is_reported_for(@"
namespace log4net
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
namespace log4net
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
namespace log4net
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
namespace log4net
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
namespace log4net
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
        public void No_issue_is_reported_for_Log_call_in_case_section() => No_issue_is_reported_for(@"
namespace log4net
{
    public static class Constants
    {
        public static class ILog
        {
            public const string IsDebugEnabled = ""Some text"";
        }
    }

    public class TestMe
    {
        public void DoSomething(string text)
        {
            switch (text)
            {
                case ""something"":
                case Constants.ILog.IsDebugEnabled:
                case ""something else"":
                {
                    return;
                }
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Log_call_in_Moq_call() => No_issue_is_reported_for(@"
using Moq;

namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }

        void Debug(string text);
        void Info(string text);
        void Warn(string text);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            var mock = new Mock<ILog>();

            mock.Verify(_ => _.IsInfoEnabled), Times.Once);
            mock.Verify(_ => _.IsDebugEnabled), Times.Never);
            mock.Verify(_ => _.IsWarnEnabled, Times.Never);
            mock.Verify(_ => _.Info(""some text"")), Times.Once);
            mock.Verify(_ => _.Debug(""some text"")), Times.Never);
            mock.Verify(_ => _.Warn(""some text""), Times.Never);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Log_call_as_if_condition_when_preceded_by_code() => An_issue_is_reported_for(@"
namespace log4net
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
            something = true;
            if (Log.IsDebugEnabled) Log.Debug();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Log_call_as_if_condition_when_followed_by_code() => An_issue_is_reported_for(@"
namespace log4net
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
            if (Log.IsDebugEnabled) Log.Debug();
            something = true;
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Log_call_followed_by_if_block() => An_issue_is_reported_for(@"
namespace log4net
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
namespace log4net
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
namespace log4net
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
        public void No_issue_is_reported_for_Serilog_call_followed_by_another_Serilog_call() => No_issue_is_reported_for(@"
namespace Serilog
{
    public static class Log
    {
        public static void Verbose();
        public static void Information();
        public static void Warning();
    }

    public class TestMe
    {
        public void DoSomething()
        {
            Log.Verbose();
            Log.Warning();
            Log.Information();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Serilog_call_followed_by_if_block() => An_issue_is_reported_for(@"
namespace Serilog
{
    public static class Log
    {
        public static void Verbose();
        public static void Information();
        public static void Warning();
    }

    public class TestMe
    {
        public void DoSomething(bool something)
        {
            Log.Verbose();
            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_MicrosoftLogging_call_followed_by_another_MicrosoftLogging_call() => No_issue_is_reported_for(@"
namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void Log();
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething()
        {
            _logger.Log();
            _logger.Log();
            _logger.Log();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_MicrosoftLogging_call_followed_by_if_block() => An_issue_is_reported_for(@"
namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void Log();
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething(bool something)
        {
            _logger.Log();
            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line()
        {
            const string OriginalCode = @"
namespace log4net
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
namespace log4net
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
namespace log4net
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
namespace log4net
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
namespace log4net
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
namespace log4net
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
namespace log4net
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
namespace log4net
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
namespace log4net
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
namespace log4net
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

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line_before_Serilog()
        {
            const string OriginalCode = @"
namespace Serilog
{
    public static class Log
    {
        public static void Verbose();
        public static void Information();
        public static void Warning();
    }

    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Log.Verbose();
        }
    }
}
";

            const string FixedCode = @"
namespace Serilog
{
    public static class Log
    {
        public static void Verbose();
        public static void Information();
        public static void Warning();
    }

    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Log.Verbose();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_missing_preceding_line_before_MicrosoftLogging()
        {
            const string OriginalCode = @"
namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void Log();
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            _logger.Log();
        }
    }
}
";

            const string FixedCode = @"
namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void Log();
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            _logger.Log();
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_if_DebugEnabled_is_on_same_line_as_check_in_multiple_lines()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void DebugFormat(string format, params object[] args);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething1(bool something)
        {
            something = false;
            if (Log.IsDebugEnabled) Log.DebugFormat(""some text {0}"", something);
            something = true;
        }

        public void DoSomething2(bool something)
        {
            something = false;
            if (Log.IsDebugEnabled) Log.DebugFormat(""some text {0}"", something);
            something = true;
        }

        public void DoSomething3(bool something)
        {
            something = false;
            if (Log.IsDebugEnabled) Log.DebugFormat(""some text {0}"", something);
            something = true;
        }
    }
}
";

            const string FixedCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void DebugFormat(string format, params object[] args);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething1(bool something)
        {
            something = false;

            if (Log.IsDebugEnabled) Log.DebugFormat(""some text {0}"", something);

            something = true;
        }

        public void DoSomething2(bool something)
        {
            something = false;

            if (Log.IsDebugEnabled) Log.DebugFormat(""some text {0}"", something);

            something = true;
        }

        public void DoSomething3(bool something)
        {
            something = false;

            if (Log.IsDebugEnabled) Log.DebugFormat(""some text {0}"", something);

            something = true;
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_6001_LogStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_6001_LogStatementSurroundedByBlankLinesAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_6001_CodeFixProvider();
    }
}