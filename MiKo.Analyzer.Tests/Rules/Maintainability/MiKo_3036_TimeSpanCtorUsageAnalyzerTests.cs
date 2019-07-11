using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3036_TimeSpanCtorUsageAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_with_correct_TimeSpan_usage() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TimeSpan DoSomething()
        {
            return TimeSpan.FromDays(4);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_body_with_correct_TimeSpan_usage() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TimeSpan DoSomething() => TimeSpan.FromDays(4);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_wrong_TimeSpan_usage() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TimeSpan DoSomething()
        {
            return new TimeSpan(4);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_body_with_wrong_TimeSpan_usage() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething() => new TimeSpan(4);
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3036_TimeSpanCtorUsageAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3036_TimeSpanCtorUsageAnalyzer();
    }
}