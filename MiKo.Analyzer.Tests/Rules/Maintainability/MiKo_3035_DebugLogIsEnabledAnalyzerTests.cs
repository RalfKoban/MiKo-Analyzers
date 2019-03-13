
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public class MiKo_3035_DebugLogIsEnabledAnalyzerTests : CodeFixVerifier
    {
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
        public void No_issue_is_reported_for_call_in_method_in_block_in_if_statement([Values("Debug", "DebugFormat")] string method) => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                Log." + method + @"();
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_method_without_block_in_if_statement([Values("Debug", "DebugFormat")] string method) => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
                Log." + method + @"();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_ctor_in_block_in_if_statement([Values("Debug", "DebugFormat")] string method) => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private static ILog Log = null;

        public TestMe()
        {
            if (Log.IsDebugEnabled)
            {
                Log." + method + @"();
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_call_in_ctor_without_block_in_if_statement([Values("Debug", "DebugFormat")] string method) => No_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private static ILog Log = null;

        public TestMe()
        {
            if (Log.IsDebugEnabled)
                Log." + method + @"();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_method_body_without_IsDebugEnabled([Values("Debug", "DebugFormat")] string method) => An_issue_is_reported_for(@"
namespace Bla
{
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
        public void An_issue_is_reported_for_call_in_method_expression_body_without_IsDebugEnabled([Values("Debug", "DebugFormat")] string method) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething() => Log." + method + @"();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_call_in_ctor_body_without_IsDebugEnabled([Values("Debug", "DebugFormat")] string method) => An_issue_is_reported_for(@"
namespace Bla
{
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
        public void An_issue_is_reported_for_call_in_ctor_expression_body_without_IsDebugEnabled([Values("Debug", "DebugFormat")] string method) => An_issue_is_reported_for(@"
namespace Bla
{
    public class TestMe
    {
        private static ILog Log = null;

        public TestMe() => Log." + method + @"();
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3035_DebugLogIsEnabledAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3035_DebugLogIsEnabledAnalyzer();
    }
}