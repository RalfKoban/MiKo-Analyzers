using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3064_LogMessagesContainsNtContractionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Methods = ["Debug", "Info", "Error", "Warn", "Fatal", "DebugFormat", "InfoFormat", "ErrorFormat", "WarnFormat", "FatalFormat"];

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
        public void No_issue_is_reported_for_calls_with_no_contraction_([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
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
        public void An_issue_is_reported_for_call_in_ctor_with_contraction_(
                                                                        [ValueSource(nameof(WrongContractionPhrases))] string contraction,
                                                                        [ValueSource(nameof(Methods))] string method)
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

        public TestMe()
        {
            Log." + method + @"(""some " + contraction + @" text"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_interpolated_call_in_ctor_with_contraction_(
                                                                                     [ValueSource(nameof(WrongContractionPhrases))] string contraction,
                                                                                     [ValueSource(nameof(Methods))] string method)
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

        public TestMe(int i)
        {
            Log." + method + @"($""some " + contraction + @" text for {i}"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_method_with_contraction_(
                                                                          [ValueSource(nameof(WrongContractionPhrases))] string contraction,
                                                                          [ValueSource(nameof(Methods))] string method)
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

        public void DoSomething()
        {
            Log." + method + @"(""some " + contraction + @" text"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_interpolated_call_in_method_with_contraction_(
                                                                                       [ValueSource(nameof(WrongContractionPhrases))] string contraction,
                                                                                       [ValueSource(nameof(Methods))] string method)
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

        public void DoSomething(int i)
        {
            Log." + method + @"($""some " + contraction + @" text for {i}"");
        }
    }
}
");

        [Test]
        public void Code_gets_fixed_for_call_in_method_with_contraction_([ValueSource(nameof(WrongContractionPhrases))] string wrongPhrase)
        {
            const string Template = @"
using System;

namespace log4net
{
    public interface ILog
    {
        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            Log.Debug(""some ### text"");
        }
    }
}
";
            VerifyCSharpFix(Template.Replace("###", wrongPhrase), Template.Replace("###", ContractionMap[wrongPhrase]));
        }

        [Test]
        public void Code_gets_fixed_for_interpolated_call_in_method_with_contraction_after_interpolate_value_([ValueSource(nameof(WrongContractionPhrases))] string wrongPhrase)
        {
            const string Template = @"
using System;

namespace log4net
{
    public interface ILog
    {
        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int i)
        {
            Log.Debug($""some text for {i} that ### work"");
        }
    }
}
";
            VerifyCSharpFix(Template.Replace("###", wrongPhrase), Template.Replace("###", ContractionMap[wrongPhrase]));
        }

        [Test]
        public void Code_gets_fixed_for_interpolated_call_in_method_with_contraction_before_interpolate_value_([ValueSource(nameof(WrongContractionPhrases))] string wrongPhrase)
        {
            const string Template = @"
using System;

namespace log4net
{
    public interface ILog
    {
        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(int i)
        {
            Log.Debug($""some ### text for {i}"");
        }
    }
}
";
            VerifyCSharpFix(Template.Replace("###", wrongPhrase), Template.Replace("###", ContractionMap[wrongPhrase]));
        }

        protected override string GetDiagnosticId() => MiKo_3064_LogMessagesContainsNtContractionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3064_LogMessagesContainsNtContractionAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3064_CodeFixProvider();
    }
}