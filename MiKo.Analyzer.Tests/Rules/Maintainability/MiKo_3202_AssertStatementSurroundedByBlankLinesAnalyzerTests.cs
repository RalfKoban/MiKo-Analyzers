using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_Assertion_followed_by_another_Assertion() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            Assert.IsTrue(true);
            Assert.IsFalse(false);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Assertion_followed_by_if_block_separated_by_blank_line() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            Assert.IsTrue(true);

            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Assertion_preceded_by_if_block_separated_by_blank_line() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Assert.IsTrue(true);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Assertion_followed_by_if_block() => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            Assert.IsTrue(true);
            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Assertion_preceded_by_if_block() => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Assert.IsTrue(true);
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzer();
    }
}