using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3061_LoggerHasCategoryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_Logger_usage_within_method() => No_issue_is_reported_for(@"
using System;
using System.Diagnostics;

public class TestMe
{
    public void DoSomething()
    {
        Console.WriteLine(""bla"");
    }
}
");

        [Test]
        public void No_issue_is_reported_for_GetLogger_but_non_Logger_usage_within_method() => No_issue_is_reported_for(@"
using System;
using System.Diagnostics;

public class TestMe
{
    public void DoSomething()
    {
        GetLogger(42);
    }

    private int GetLogger(int i) => i;
}
");

        [Test]
        public void No_issue_is_reported_for_Logger_with_string_usage_within_method() => No_issue_is_reported_for(@"
using System;

namespace log4net
{
    public interface ILog
    {
    }

    public static class LogManager
    {
        public static ILog GetLogger(string category) => null;

        public static ILog GetLogger(Type category) => null;
    }
}

public class TestMe
{
    public void DoSomething(int i)
    {
        var log = LogManager.GetLogger(""bla"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Logger_with_type_usage_within_method() => An_issue_is_reported_for(@"
using System;
using System.Reflection;

namespace log4net
{
    public interface ILog
    {
    }

    public static class LogManager
    {
        public static ILog GetLogger(string category) => null;

        public static ILog GetLogger(Type category) => null;
    }
}

public class TestMe
{
    public void DoSomething(int i)
    {
        var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Logger_with_string_usage_at_field_assignment() => No_issue_is_reported_for(@"
using System;
using System.Reflection;

namespace log4net
{
    public interface ILog
    {
    }

    public static class LogManager
    {
        public static ILog GetLogger(string category) => null;

        public static ILog GetLogger(Type category) => null;
    }
}

public class TestMe
{
    public void DoSomething(int i)
    {
    }

    private static readonly ILog Log = LogManager.GetLogger(""bla"");
}
");

        [Test]
        public void An_issue_is_reported_for_Logger_with_type_usage_at_field_assignment() => An_issue_is_reported_for(@"
using System;
using System.Reflection;

namespace log4net
{
    public interface ILog
    {
    }

    public static class LogManager
    {
        public static ILog GetLogger(string category) => null;

        public static ILog GetLogger(Type category) => null;
    }
}

public class TestMe
{
    public void DoSomething(int i)
    {
    }

    private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
}
");

        protected override string GetDiagnosticId() => MiKo_3061_LoggerHasCategoryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3061_LoggerHasCategoryAnalyzer();
    }
}