﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

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

        protected override string GetDiagnosticId() => MiKo_5001_DebugLogIsEnabledAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5001_DebugLogIsEnabledAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5001_CodeFixProvider();
    }
}