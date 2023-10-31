using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3116_TestSetUpMethodsContainCodeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_empty_non_test_setup_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_empty_non_test_setup_method() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            var x = 0;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_empty_test_or_teardown_method() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething() { }

        [TearDown]
        public void CleanupTest() { }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_setup_method_that_contains_only_a_single_line_comment() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            // some comment
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_setup_method_that_contains_only_a_multi_line_comment() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            /* some comment */
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_empty_test_setup_method() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [SetUp]
        public void DoSomething()
        {
            var x = 0;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_abstract_test_setup_method() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [SetUp]
        public abstract void DoSomething();
    }
}
");

        [Test]
        public void No_issue_is_reported_for_expression_body_test_setup_method() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [SetUp]
        public void DoSomething() => Assert.Fail();
    }
}
");

        [Test]
        public void An_issue_is_reported_for_empty_test_setup_method() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [SetUp]
        public void DoSomething() { }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_setup_method_that_contains_only_a_single_line_comment() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [SetUp]
        public void DoSomething()
        {
            // some comment
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_test_setup_method_that_contains_only_a_multi_line_comment() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [SetUp]
        public void DoSomething()
        {
            /* some comment */
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3116_TestSetUpMethodsContainCodeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3116_TestSetUpMethodsContainCodeAnalyzer();
    }
}