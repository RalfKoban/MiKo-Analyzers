using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5001_DebugLogIsEnabledAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Methods = { "Debug", "DebugFormat" };

        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
namespace log4net
{
    public class TestMe
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
namespace log4net
{
    public class TestMe
    {
        public void DoSomething()
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_in_block_in_if_statement_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                Log." + method + @"();
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_in_block_in_if_statement_with_AND_condition_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool flag)
        {
            if (flag && Log.IsDebugEnabled)
            {
                Log." + method + @"();
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_in_block_in_deeply_nested_foreach_statement_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(IEnumerable<string> items)
        {
            if (Log.IsDebugEnabled)
            {
                Log." + method + @"();

                foreach (var item in item)
                {
                    Log." + method + @"();
                }
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_in_block_in_deeply_nested_if_statement_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool flag)
        {
            if (Log.IsDebugEnabled)
            {
                if (flag)
                {
                    Log." + method + @"();
                }
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_in_else_block_in_deeply_nested_if_statement_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool flag)
        {
            if (Log.IsDebugEnabled)
            {
                if (flag)
                {
                }
                else
                {
                    Log." + method + @"();
                }
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_without_block_in_if_statement_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
                Log." + method + @"();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_body_without_non_existing_IsDebugEnabled_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            Log." + method + @"();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_in_block_in_complex_if_statement_with_interpolated_debug_message_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(Guid someGuid, bool flag)
        {
            if (Log.IsDebugEnabled && someGuid != Guid.Empty && !flag)
            {
                var message = $""some message for {someGuid}."";
                Log." + method + @"(message);
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_ctor_in_block_in_if_statement_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe()
        {
            if (Log.IsDebugEnabled)
            {
                Log." + method + @"();
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_ctor_without_block_in_if_statement_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe()
        {
            if (Log.IsDebugEnabled)
                Log." + method + @"();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_unrelated_call_in_ctor_expression_body() => No_issue_is_reported_for(@"
namespace log4net
{
    public enum TraceLevel
    {
        Debug = 0,
    }

    public class TestMe
    {
        private TraceLevel _level;

        public TestMe() : this(TraceLevel.Debug)
        { }

        public TestMe(TraceLevel level) => _level = level;
    }
}
");

        [Test]
        public void No_issue_is_reported_for_unrelated_method_call() => No_issue_is_reported_for(@"
namespace log4net
{
    public class TestMe
    {
        public TestMe()
        { }

        public void DoSomething() => Debug();

        public void Debug()
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Moq_setup_or_verification_call() => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }
}

namespace Moq
{
    public class Mock<T>
    {
    }
}

namespace Bla
{
    using log4net;
    using Moq;

    public class TestMe
    {
        public void DoSomething()
        {
            var mock = new Mock<ILog>();

            mock.Setup(_ => _.Debug());

            mock.Verify(_ => _.Debug(), Times.Never);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_method_body_without_IsDebugEnabled_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            Log." + method + @"();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_method_expression_body_without_IsDebugEnabled_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething() => Log." + method + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_ctor_body_without_IsDebugEnabled_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe()
        {
            Log." + method + @"();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_ctor_expression_body_without_IsDebugEnabled_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe() => Log." + method + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_method_in_block_in_if_statement_with_OR_condition_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool flag)
        {
            if (flag || Log.IsDebugEnabled)
            {
                Log." + method + @"();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_method_in_else_block_of_if_statement_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();

        void Info();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                // missing log call
            }
            else
            {
                Log." + method + @"();
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_local_function_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool flag)
        {
            DoSomethingCore();

            void DoSomethingCore()
            {
                Log." + method + @"();
            }
        }
    }
}
");

        [Test]
        public void Code_gets_fixed()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe()
        {
            Log.Debug(""something"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""something"");
            }
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_when_surrounded_by_other_statements()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            something = true;
            Log.Debug(""something"");
            something = false;
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

        void Debug(string text);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            something = true;
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""something"");
            }
            something = false;
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_local_function()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool flag)
        {
            DoSomethingCore();

            void DoSomethingCore()
            {
                Log.Debug(""something"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool flag)
        {
            DoSomethingCore();

            void DoSomethingCore()
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug(""something"");
                }
            }
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interpolated_string()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe(bool b)
        {
            Log.Debug($""something: {b}"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe(bool b)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($""something: {b}"");
            }
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interpolated_string_on_static_helper()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public static class TestContext
    {
        public static ILog Log;
    }

    public class TestMe
    {
        public TestMe(bool b)
        {
            TestContext.Log.Debug($""something: {b}"");
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

        void Debug(string text);
    }

    public static class TestContext
    {
        public static ILog Log;
    }

    public class TestMe
    {
        public TestMe(bool b)
        {
            if (TestContext.Log.IsDebugEnabled)
            {
                TestContext.Log.Debug($""something: {b}"");
            }
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_interpolated_string_on_deep_static_helper()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class LogContext
    {
        public readonly ILog Log;
    }

    public static class TestContext
    {
        public static readonly LogContext Context;
    }

    public class TestMe
    {
        public TestMe(bool b)
        {
            TestContext.Context.Log.Debug($""something: {b}"");
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

        void Debug(string text);
    }

    public class LogContext
    {
        public readonly ILog Log;
    }

    public static class TestContext
    {
        public static readonly LogContext Context;
    }

    public class TestMe
    {
        public TestMe(bool b)
        {
            if (TestContext.Context.Log.IsDebugEnabled)
            {
                TestContext.Context.Log.Debug($""something: {b}"");
            }
        }
    }
}
";
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_in_callback()
        {
            const string OriginalCode = @"
using System.Threading;

namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Token.Register(() => Log.DebugFormat(""some text""));
        }
    }
}
";

            const string FixedCode = @"
using System.Threading;

namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Token.Register(() => { if (Log.IsDebugEnabled) { Log.DebugFormat(""some text""); } });
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_non_consecutive_calls()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            Log.Debug(""some text"");
            Log.Error(""some more text"");
            Log.Debug(""even some more text"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some text"");
            }
            Log.Error(""some more text"");
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""even some more text"");
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_consecutive_calls()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            Log.Debug(""some text"");
            Log.Debug(""some more text"");
            Log.Debug(""even some more text"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some text"");
                Log.Debug(""some more text"");
                Log.Debug(""even some more text"");
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_consecutive_call_when_first_is_already_part_of_an_IsDebugEnabled_with_block_as_body()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some text"");
            }

            Log.Debug(""some more text"");
            Log.Debug(""even some more text"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some text"");
                Log.Debug(""some more text"");
                Log.Debug(""even some more text"");
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_consecutive_call_when_first_is_already_part_of_an_IsDebugEnabled_without_block_as_body()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled) Log.Debug(""some text"");

            Log.Debug(""some more text"");
            Log.Debug(""even some more text"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some text"");
                Log.Debug(""some more text"");
                Log.Debug(""even some more text"");
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_consecutive_call_when_last_is_already_part_of_an_IsDebugEnabled_with_block_as_body()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            Log.Debug(""some text"");

            // some comment
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some more text"");
                Log.Debug(""even some more text"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            // some comment
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some text"");
                Log.Debug(""some more text"");
                Log.Debug(""even some more text"");
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_consecutive_call_when_last_is_already_part_of_an_IsDebugEnabled_without_block_as_body()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            Log.Debug(""some text"");

            // some comment
            if (Log.IsDebugEnabled) Log.Debug(""some more text"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            // some comment
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some text"");
                Log.Debug(""some more text"");
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_consecutive_calls_when_last_is_already_part_of_an_IsDebugEnabled_without_block_as_body()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            Log.Debug(""some text"");
            Log.Debug(""some more text"");

            if (Log.IsDebugEnabled) Log.Debug(""some even more text"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some text"");
                Log.Debug(""some more text"");
                Log.Debug(""some even more text"");
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_consecutive_calls_when_first_and_last_are_already_part_of_an_IsDebugEnabled_without_block_as_body()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled) Log.Debug(""some text"");

            Log.Debug(""some more text"");

            if (Log.IsDebugEnabled) Log.Debug(""some even more text"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some text"");
                Log.Debug(""some more text"");
                Log.Debug(""some even more text"");
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_consecutive_calls_when_all_except_one_are_already_part_of_an_IsDebugEnabled_without_block_as_body()
        {
            const string OriginalCode = @"
namespace log4net
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled) Log.Debug(""some text"");

            Log.Debug(""some more text"");

            if (Log.IsDebugEnabled) Log.Debug(""some even more text"");

            if (Log.IsDebugEnabled) Log.Debug(""still some more text"");
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

        void Debug(string text);
    }

    public class TestMe
    {
        private ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(""some text"");
                Log.Debug(""some more text"");
                Log.Debug(""some even more text"");
                Log.Debug(""still some more text"");
            }
        }
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_5001_DebugLogIsEnabledAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5001_DebugLogIsEnabledAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5001_CodeFixProvider();
    }
}