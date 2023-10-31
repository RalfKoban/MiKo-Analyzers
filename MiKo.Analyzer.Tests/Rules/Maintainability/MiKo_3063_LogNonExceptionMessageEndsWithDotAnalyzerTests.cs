using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3063_LogNonExceptionMessageEndsWithDotAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Methods = { "Debug", "Info", "Error", "Warn", "Fatal", "DebugFormat", "InfoFormat", "ErrorFormat", "WarnFormat", "FatalFormat" };

        [Test]
        public void An_issue_is_reported_for_call_in_ctor_body_with_no_dot_message_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
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
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_ctor_expression_body_with_no_dot_message_([ValueSource(nameof(Methods))] string method)
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

        public TestMe() => Log." + method + @"(""some text"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_method_expression_body_with_interpolated_message_and_no_dot_at_end() => An_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        void Error();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int i) => Log.Error($""some text for {i}   "");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_method_expression_body_with_interpolated_message_with_interpolation_at_end() => An_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        void Error();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int i) => Log.Error($""some text for {i}"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_method_expression_body_with_no_dot_message_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
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

        public void DoSomething() => Log." + method + @"(""some text"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_DebugFormat_call_in_method_body_with_IFormatProvider_and_no_dot_message() => An_issue_is_reported_for(@"
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

        public void DoSomething(int i)
        {
            Log.DebugFormat((IFormatProvider)null, ""something went wrong"", i);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_ctor_body_with_colon_message_and_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
            Log." + method + @"(""some text:"", ex);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_ctor_body_with_dot_message_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
            Log." + method + @"(""some text."");
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_ctor_body_without_message_and_without_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_call_in_ctor_expression_body_with_colon_message_and_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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

        public TestMe(Exception ex) => Log." + method + @"(""some text:"", ex);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_ctor_expression_body_with_dot_message_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe() => Log." + method + @"(""some text."");
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_ctor_expression_body_without_message_and_without_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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

        [Test]
        public void No_issue_is_reported_for_call_in_method_body_with_colon_message_and_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
            Log." + method + @"(""some call:"", ex);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_body_with_dot_message_([ValueSource(nameof(Methods))] string method, [Values("some text:", "some text.")] string text) => No_issue_is_reported_for(@"
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
            Log." + method + "(" + text + @");
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_body_without_message_and_without_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_call_in_method_expression_body_with_colon_interpolated_message_and_exception_argument() => No_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        void Error();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int i, Exception ex) => Log.Error($""some text for {i}: "", ex);
    }
}
");

        [Test,]
        public void No_issue_is_reported_for_call_in_method_expression_body_with_colon_message_and_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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

        public void DoSomething(Exception ex) => Log." + method + @"(""some text:"", ex);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_expression_body_with_dot_message_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething() => Log." + method + @"(""some text."");
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_expression_body_with_interpolated_message_with_specific_ending_([Values(".", ":")] string ending) => No_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        void Error();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int i) => Log.Error($""some text for {i}" + ending + @""");
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_expression_body_with_interpolated_message_with_interpolation_at_end_and_exception_argument() => No_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
        void Error();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int i, Exception ex) => Log.Error($""some text for {i}"", ex);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_expression_body_without_message_and_without_exception_argument_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_DebugFormat_call_in_method_body_with_IFormatProvider_and_colon_message_and_exception_argument() => No_issue_is_reported_for(@"
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
            Log.DebugFormat((IFormatProvider)null, ""something went wrong:"", ex);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_DebugFormat_call_in_method_body_with_IFormatProvider_and_dot_message() => No_issue_is_reported_for(@"
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

        public void DoSomething(int i)
        {
            Log.DebugFormat((IFormatProvider)null, ""something went wrong."", i);
        }
    }
}
");

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

        [TestCase("some text", "some text.")]
        [TestCase("some text ", "some text.")]
        [TestCase("some text?", "some text.")]
        [TestCase("some text!", "some text.")]
        public void Code_gets_fixed_for_non_interpolated_log_message_(string originalText, string fixedText)
        {
            const string Template = @"
using System;

namespace log4net
{
    public interface ILog
    {
        void Error(string, Exception);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            Log.Error(""###"");
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", originalText), Template.Replace("###", fixedText));
        }

        [Test]
        public void Code_gets_fixed_for_interpolated_log_message()
        {
            const string Template = @"
using System;

namespace log4net
{
    public interface ILog
    {
        void Error(string, Exception);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int i)
        {
            Log.Error(###);
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", "$\"some {i} text\""), Template.Replace("###", "$\"some {i} text.\""));
        }

        [Test]
        public void Code_gets_fixed_for_interpolated_log_message_with_interpolation_at_end()
        {
            const string Template = @"
using System;

namespace log4net
{
    public interface ILog
    {
        void Error(string, Exception);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int i)
        {
            Log.Error(###);
        }
    }
}
";

            VerifyCSharpFix(Template.Replace("###", "$\"some text {i}\""), Template.Replace("###", "$\"some text {i}.\""));
        }

        protected override string GetDiagnosticId() => MiKo_3063_NonExceptionLogMessageEndsWithDotAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3063_NonExceptionLogMessageEndsWithDotAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3063_CodeFixProvider();
    }
}