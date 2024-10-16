using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3065_MicrosoftLoggingMessagesDoNotUseInterpolatedStringsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] LogForNetMethods = ["Debug", "Info", "Error", "Warn", "Fatal", "DebugFormat", "InfoFormat", "ErrorFormat", "WarnFormat", "FatalFormat"];
        private static readonly string[] Methods = ["BeginScope", "Log", "LogCritical", "LogDebug", "LogError", "LogInformation", "LogTrace", "LogWarning"];

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
        public void No_issue_is_reported_for_non_logging_calls_with_interpolation() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public TestMe(int i) => SomeMethod($""some text for {i}"");

    public void DoSomething(int i) => SomeMethod($""some text for {i}"");

    public void SomeMethod(string text) { }
}
");

        [Test]
        public void No_issue_is_reported_for_log4net_calls_with_interpolation_([ValueSource(nameof(LogForNetMethods))] string method) => No_issue_is_reported_for(@"
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

        public TestMe()
        {
            Log." + method + @"(""some text"");
        }

        public TestMe(Exception ex)
        {
            Log." + method + @"(""some text"", ex);
        }

        public void DoSomething(int i) => Log." + method + @"($""some text for {i}"");

        public void DoSomething(int i, Exception ex) => Log." + method + @"($""some text for {i}"", ex);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Microsoft_logging_call_without_interpolation_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void " + method + @"(string message, params object[] args);
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething()
        {
            _logger." + method + @"(""some text"");
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Microsoft_logging_call_with_interpolation_as_argument_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void " + method + @"(string message, params object[] args);
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething(int i)
        {
            _logger." + method + @"(""some text for {i}"", $""some text for {i}"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Microsoft_logging_call_with_interpolation_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void " + method + @"(string message, params object[] args);
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething(int i)
        {
            _logger." + method + @"($""some text for {i}"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Microsoft_logging_call_with_interpolation_and_format_provider_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void " + method + @"(string message, params object[] args);
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething(int i)
        {
            _logger." + method + @"($""some text for {i:D}"");
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_Microsoft_logging_call_with_interpolation_([ValueSource(nameof(Methods))] string method)
        {
            var originalCode = @"
using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void " + method + @"(string message, params object?[] args);
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething(int x, int y, int z)
        {
            _logger." + method + @"($""some text for {x}, {y} and {z}"");
        }
    }
}
";

            var fixedCode = @"
using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void " + method + @"(string message, params object?[] args);
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething(int x, int y, int z)
        {
            _logger." + method + @"(""some text for {x}, {y} and {z}"", x, y, z);
        }
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_Microsoft_logging_call_with_interpolation_and_format_provider_([ValueSource(nameof(Methods))] string method)
        {
            var originalCode = @"
using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void " + method + @"(string message, params object?[] args);
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething(int x, int y, int z)
        {
            _logger." + method + @"($""some text for {x:D}, {y:G} and {z:C}"");
        }
    }
}
";

            var fixedCode = @"
using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        public void " + method + @"(string message, params object?[] args);
    }

    public class TestMe
    {
        private ILogger _logger;

        public void DoSomething(int x, int y, int z)
        {
            _logger." + method + @"(""some text for {x}, {y} and {z}"", x.ToString(""D""), y.ToString(""G""), z.ToString(""C""));
        }
    }
}
";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        //// TODO RKN: Add tests for 'string.Format'

        protected override string GetDiagnosticId() => MiKo_3065_MicrosoftLoggingMessagesDoNotUseInterpolatedStringsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3065_MicrosoftLoggingMessagesDoNotUseInterpolatedStringsAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3065_CodeFixProvider();
    }
}