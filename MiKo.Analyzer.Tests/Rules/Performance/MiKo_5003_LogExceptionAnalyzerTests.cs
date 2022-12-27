using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5003_LogExceptionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Methods = { "Debug", "Info", "Error", "Warn", "Fatal", "DebugFormat", "InfoFormat", "ErrorFormat", "WarnFormat", "FatalFormat" };

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
        public void No_issue_is_reported_for_call_in_method_body_without_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_call_in_method_expression_body_without_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
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
        public void No_issue_is_reported_for_call_in_ctor_body_without_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
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
        public void No_issue_is_reported_for_call_in_ctor_expression_body_without_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe() => Log." + method + @"();
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_call_in_method_body_with_exception_argument_(
                                                                                    [ValueSource(nameof(Methods))] string method,
                                                                                    [Values("ex", "ex.ToString()")] string call)
            => An_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(Exception ex)
        {
            Log." + method + "(" + call + @");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DebugFormat_call_in_method_body_with_exception_argument_as_formattable() => An_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        void DebugFormat(string format, object arg);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(Exception ex)
        {
            Log.DebugFormat(""something went wrong"", ex);
        }
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_call_in_method_expression_body_with_exception_argument_(
                                                                                        [ValueSource(nameof(Methods))] string method,
                                                                                        [Values("ex", "ex.ToString()")] string call)
            => An_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(Exception ex) => Log." + method + "(" + call + @");
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_call_in_ctor_body_with_exception_argument_(
                                                                                    [ValueSource(nameof(Methods))] string method,
                                                                                    [Values("ex", "ex.ToString()")] string call)
            => An_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe(Exception ex)
        {
            Log." + method + "(" + call + @");
        }
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_call_in_ctor_expression_body_with_exception_argument_(
                                                                                            [ValueSource(nameof(Methods))] string method,
                                                                                            [Values("ex", "ex.ToString()")] string call)
            => An_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe(Exception ex) => Log." + method + "(" + call + @");
    }
}
");

        protected override string GetDiagnosticId() => MiKo_5003_LogExceptionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5003_LogExceptionAnalyzer();
    }
}