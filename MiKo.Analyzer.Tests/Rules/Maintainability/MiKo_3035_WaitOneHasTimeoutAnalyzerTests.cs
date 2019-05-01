using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3035_WaitOneHasTimeoutAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_with_correct_WaitOne_usage() => No_issue_is_reported_for(@"
using System;
using System.Threading;

namespace Bla
{
    public class TestMe
    {
        private WaitHandle h = null;
            
        public void DoSomething()
        {
            h.WaitOne(TimeSpan.Zero);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_body_with_correct_WaitOne_usage() => No_issue_is_reported_for(@"
using System;
using System.Threading;

namespace Bla
{
    public class TestMe
    {
        private WaitHandle h = null;
            
        public bool DoSomething() => h.WaitOne(TimeSpan.Zero);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_with_wrong_WaitOne_usage() => An_issue_is_reported_for(@"
using System;
using System.Threading;

namespace Bla
{
    public class TestMe
    {
        private WaitHandle h = null;
            
        public void DoSomething()
        {
            h.WaitOne();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_method_body_with_wrong_WaitOne_usage() => An_issue_is_reported_for(@"
using System;
using System.Threading;

namespace Bla
{
    public class TestMe
    {
        private WaitHandle h = null;
            
        public bool DoSomething() => h.WaitOne();
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3035_WaitOneHasTimeoutAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3035_WaitOneHasTimeoutAnalyzer();
    }
}