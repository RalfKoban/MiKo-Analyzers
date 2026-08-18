using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed partial class MiKo_3130_AssertFailInsideIfBlockAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_Assert_Fail_in_method_body() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest()
        {
            Assert.Fail(""some failure"");
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Assert_Fail_in_default_block_of_switch() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest(StringComparison comparison)
        {
            switch (comparison)
            {
                case StringComparison.Ordinal: break;
                case StringComparison.OrdinalIgnoreCase: break;
                default:
                    Assert.Fail(""some failure"");
                    break;
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Assert_Fail_in_if_block_when_preceded_by_some_assignments() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest(int value)
        {
            if (value is 42)
            {
                Assert.That(value, Is.EqualTo(42));

                Assert.Fail(""some failure"");
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Assert_Fail_in_else_block_when_preceded_by_some_assignments() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest(int value)
        {
            if (value != 42)
            {
            }
            else
            {
                Assert.That(value, Is.EqualTo(42));

                Assert.Fail(""some failure"");
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Assert_Fail_as_only_statement_in_if_statement() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest(int value)
        {
            if (value is 42)
                Assert.Fail(""some failure"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Assert_Fail_as_only_statement_in_if_block() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest(int value)
        {
            if (value is 42)
            {
                Assert.Fail(""some failure"");
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Assert_Fail_as_only_statement_in_else_statement() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest(int value)
        {
            if (value is 42)
            {
                Assert.That(value, Is.EqualTo(42));
            }
            else
                Assert.Fail(""some failure"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Assert_Fail_as_only_statement_in_else_block() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void SomeTest(int value)
        {
            if (value is 42)
            {
                Assert.That(value, Is.EqualTo(42));
            }
            else
            {
                Assert.Fail(""some failure"");
            }
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3130_AssertFailInsideIfBlockAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3130_AssertFailInsideIfBlockAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3130_CodeFixProvider();
    }
}