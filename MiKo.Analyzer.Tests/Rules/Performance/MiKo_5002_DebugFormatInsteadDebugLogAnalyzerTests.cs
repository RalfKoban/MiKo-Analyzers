using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [TestFixture]
    public sealed class MiKo_5002_DebugFormatInsteadDebugLogAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Methods = ["Debug", "Info", "Error", "Warn", "Fatal"];

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
        public void No_issue_is_reported_for_non_formatting_call_in_method_body_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_non_formatting_call_in_method_expression_body_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_non_formatting_call_in_ctor_body_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_non_formatting_call_in_ctor_expression_body_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_formatting_call_in_Moq_call_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
using Moq;

namespace log4net
{
    public interface ILog
    {
        void " + method + @"Format(string text);
    }

    public class TestMe
    {
        public void DoSomething()
        {
            var mock = new Mock<ILog>();

            mock.Verify(_ => _." + method + @"Format(""Some text""), Times.Once);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_formatting_call_in_method_body_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
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
            Log." + method + @"Format(""a"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_formatting_call_in_method_expression_body_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething() => Log." + method + @"Format(""a"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_formatting_call_in_ctor_body_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
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
            Log." + method + @"Format(""a"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_formatting_call_in_ctor_expression_body_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace log4net
{
    public interface ILog
    {
        void " + method + @"();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe() => Log." + method + @"Format(""a"");
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

        void DebugFormat(string text);
    }

    public class TestMe
    {
        private static ILog Log = null;

        public TestMe()
        {
            Log.DebugFormat(""something"");
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

        void DebugFormat(string text);
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
            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_5002_DebugFormatInsteadDebugLogAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_5002_DebugFormatInsteadDebugLogAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_5002_CodeFixProvider();
    }
}