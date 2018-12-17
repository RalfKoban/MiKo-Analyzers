using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3060_DebugTraceAssertAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_non_Assert_usage_method([Values("Debug", "Trace")] string className) => No_issue_is_reported_for(@"
using System;
using System.Diagnostics;

public class TestMe
{
    public void DoSomething()
    {
        " + className + @".WriteLine(""bla"");
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Assert_usage_method([Values("Debug", "Trace")] string className) => An_issue_is_reported_for(@"
using System;
using System.Diagnostics;

public class TestMe
{
    public void DoSomething(int i)
    {
        " + className + @".Assert(i != 42);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_fully_qualified_Assert_usage_method([Values("Debug", "Trace")] string className) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int i)
    {
        System.Diagnostics." + className + @".Assert(i != 42);
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3060_DebugTraceAssertAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3060_DebugTraceAssertAnalyzer();
    }
}