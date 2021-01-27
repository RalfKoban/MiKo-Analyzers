using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Assertions =
            {
                nameof(Assert),
                nameof(Assume),
                nameof(CollectionAssert),
                nameof(DirectoryAssert),
                nameof(FileAssert),
                nameof(StringAssert),
            };

        [Test]
        public void No_issue_is_reported_for_Assertion_followed_by_another_Assertion_([ValueSource(nameof(Assertions))]string assertion) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            " + assertion + @".IsTrue(true);
            " + assertion + @".IsFalse(false);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Assertion_followed_by_if_block_separated_by_blank_line_([ValueSource(nameof(Assertions))] string assertion) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            " + assertion + @".IsTrue(true);

            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Assertion_preceded_by_if_block_separated_by_blank_line_([ValueSource(nameof(Assertions))] string assertion) => No_issue_is_reported_for(@"
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

            " + assertion + @".IsTrue(true);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Assertion_followed_by_if_block_([ValueSource(nameof(Assertions))] string assertion) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool something)
        {
            " + assertion + @".IsTrue(true);
            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Assertion_preceded_by_if_block_([ValueSource(nameof(Assertions))] string assertion) => An_issue_is_reported_for(@"
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
            " + assertion + @".IsTrue(true);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Attribute_([ValueSource(nameof(Assertions))] string assertion) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething([Values(nameof(DoSomething), nameof(TestMe))] string something)
        {
            " + assertion + @".IsTrue(true);
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3202_AssertStatementSurroundedByBlankLinesAnalyzer();
    }
}