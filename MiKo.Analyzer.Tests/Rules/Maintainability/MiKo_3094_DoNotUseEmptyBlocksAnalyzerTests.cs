using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3094_DoNotUseEmptyBlocksAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_constructor() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public TestMe()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_method_with_comment() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            // empty by default
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_with_call() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            GC.Collect();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_if_statement_with_comment() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool flag)
        {
            if (flag)
            {
                // nothing to do here
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_else_statement_with_comment() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool flag)
        {
            if (flag)
            {
                GC.Collect();
            }
            else
            {
                // nothing to do here
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_catch_block_with_comment() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool flag)
        {
            try
            {
                GC.Collect();
            }
            catch
            {
                // nothing to do here
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_method_without_comment() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_if_statement_without_comment() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool flag)
        {
            if (flag)
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_else_statement_without_comment() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool flag)
        {
            if (flag)
            {
                GC.Collect();
            }
            else
            {
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_catch_block_without_comment() => An_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool flag)
        {
            try
            {
                GC.Collect();
            }
            catch
            {
            }
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3094_DoNotUseEmptyBlocksAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3094_DoNotUseEmptyBlocksAnalyzer();
    }
}