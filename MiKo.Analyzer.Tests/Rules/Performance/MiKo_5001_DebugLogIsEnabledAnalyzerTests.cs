using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public class MiKo_5001_DebugLogIsEnabledAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Methods = { "Debug", "DebugFormat" };

        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    { }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_in_block_in_if_statement([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace Bla
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
        public void No_issue_is_reported_for_call_in_method_without_block_in_if_statement([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace Bla
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
        public void No_issue_is_reported_for_call_in_ctor_in_block_in_if_statement([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace Bla
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
        public void No_issue_is_reported_for_call_in_ctor_without_block_in_if_statement([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace Bla
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
        public void An_issue_is_reported_for_call_in_method_body_without_IsDebugEnabled([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace Bla
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
        public void An_issue_is_reported_for_call_in_method_expression_body_without_IsDebugEnabled([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace Bla
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
        public void An_issue_is_reported_for_call_in_ctor_body_without_IsDebugEnabled([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace Bla
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
        public void An_issue_is_reported_for_call_in_ctor_expression_body_without_IsDebugEnabled([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace Bla
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
        public void No_issue_is_reported_for_unrelated_call_in_ctor_expression_body() => No_issue_is_reported_for(@"
namespace Bla
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
namespace Bla
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

        protected override string GetDiagnosticId() => MiKo_5001_DebugLogIsEnabledAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5001_DebugLogIsEnabledAnalyzer();
    }
}