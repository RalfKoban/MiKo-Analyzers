﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public class MiKo_3036_DebugFormatInsteadDebugLogAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Methods = { "Debug", "Info", "Error", "Warn", "Fatal" };

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
        public void No_issue_is_reported_for_non_formatting_call_in_method_body([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace Bla
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
        public void No_issue_is_reported_for_non_formatting_call_in_method_expression_body([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace Bla
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
        public void No_issue_is_reported_for_non_formatting_call_in_ctor_body([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace Bla
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
        public void No_issue_is_reported_for_non_formatting_call_in_ctor_expression_body([ValueSource(nameof(Methods))] string method) => No_issue_is_reported_for(@"
namespace Bla
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
        public void An_issue_is_reported_for_formatting_call_in_method_body([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace Bla
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
        public void An_issue_is_reported_for_formatting_call_in_method_expression_body([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace Bla
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
        public void An_issue_is_reported_for_formatting_call_in_ctor_body([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace Bla
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
        public void An_issue_is_reported_for_formatting_call_in_ctor_expression_body([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
namespace Bla
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

        protected override string GetDiagnosticId() => MiKo_3036_DebugFormatInsteadDebugLogAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3036_DebugFormatInsteadDebugLogAnalyzer();
    }
}