using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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

        [TestCase("1")]
        [TestCase("1, 2")]
        [TestCase("1, 2, 3")]
        [TestCase("1, 2, 3, 4")]
        [TestCase("1, 2, 3, 4, 5")]
        public void An_issue_is_reported_for_method_with_wrong_TimeSpan_usage(string parameters) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TimeSpan DoSomething()
        {
            return new TimeSpan(" + parameters + @");
        }
    }
}
");

        [TestCase("1")]
        [TestCase("1, 2")]
        [TestCase("1, 2, 3")]
        [TestCase("1, 2, 3, 4")]
        [TestCase("1, 2, 3, 4, 5")]
        public void An_issue_is_reported_for_method_body_with_wrong_TimeSpan_usage(string parameters) => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public bool DoSomething() => new TimeSpan(" + parameters + @");
    }
}
");

        [TestCase("new TimeSpan(42)", "TimeSpan.FromTicks(42)")]
        [TestCase("new TimeSpan(42, 0, 0)", "TimeSpan.FromHours(42)")]
        [TestCase("new TimeSpan(0, 42, 0)", "TimeSpan.FromMinutes(42)")]
        [TestCase("new TimeSpan(0, 0, 42)", "TimeSpan.FromSeconds(42)")]
        [TestCase("new TimeSpan(42, 0, 0, 0)", "TimeSpan.FromDays(42)")]
        [TestCase("new TimeSpan(0, 42, 0, 0)", "TimeSpan.FromHours(42)")]
        [TestCase("new TimeSpan(0, 0, 42, 0)", "TimeSpan.FromMinutes(42)")]
        [TestCase("new TimeSpan(0, 0, 0, 42)", "TimeSpan.FromSeconds(42)")]
        [TestCase("new TimeSpan(42, 0, 0, 0, 0)", "TimeSpan.FromDays(42)")]
        [TestCase("new TimeSpan(0, 42, 0, 0, 0)", "TimeSpan.FromHours(42)")]
        [TestCase("new TimeSpan(0, 0, 42, 0, 0)", "TimeSpan.FromMinutes(42)")]
        [TestCase("new TimeSpan(0, 0, 0, 42, 0)", "TimeSpan.FromSeconds(42)")]
        [TestCase("new TimeSpan(0, 0, 0, 0, 42)", "TimeSpan.FromMilliseconds(42)")]
        [TestCase("new TimeSpan(42, 0, 0, 0, 0, 0)", "TimeSpan.FromDays(42)")]
        [TestCase("new TimeSpan(0, 42, 0, 0, 0, 0)", "TimeSpan.FromHours(42)")]
        [TestCase("new TimeSpan(0, 0, 42, 0, 0, 0)", "TimeSpan.FromMinutes(42)")]
        [TestCase("new TimeSpan(0, 0, 0, 42, 0, 0)", "TimeSpan.FromSeconds(42)")]
        [TestCase("new TimeSpan(0, 0, 0, 0, 42, 0)", "TimeSpan.FromMilliseconds(42)")]
        [TestCase("new TimeSpan(0, 0, 0, 0, 0, 42)", "TimeSpan.FromMicroseconds(42)", IgnoreReason = "Cannot test as this method is part of .NET 7")]
        public void Code_gets_fixed_(string originalCode, string fixedCode)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public TimeSpan DoSomething()
        {
            return ###;
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [TestCase("new TimeSpan(1, 2, 0)")]
        [TestCase("new TimeSpan(1, 2, 3)")]
        [TestCase("new TimeSpan(1, 2, 3, 0)")]
        [TestCase("new TimeSpan(1, 2, 3, 4)")]
        [TestCase("new TimeSpan(1, 2, 3, 4, 0)")]
        [TestCase("new TimeSpan(1, 2, 3, 4, 5)")]
        [TestCase("new TimeSpan(1, 2, 3, 4, 5, 0)")]
        [TestCase("new TimeSpan(1, 2, 3, 4, 5, 6)")]
        public void Code_gets_not_fixed_for_unfixable_(string code)
        {
            const string Template = @"
using System;

namespace Bla
{
    public class TestMe
    {
        public TimeSpan DoSomething()
        {
            return ###;
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", code), Template.Replace("###", code));
        }

        protected override string GetDiagnosticId() => MiKo_3036_TimeSpanCtorUsageAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3036_TimeSpanCtorUsageAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3036_CodeFixProvider();
    }
}