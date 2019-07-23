using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NCrunch.Framework;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture, Isolated]
    public sealed class MiKo_3106_TestAssertsDoNotUseOperatorsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] Operators =
            {
                "==",
                "!=",
                "<",
                "<=",
                ">",
                ">=",
            };

        private static readonly string[] Methods =
            {
                "All",
                "Any",
                "Contains",
                "Equals",
            };

        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_empty_method() => No_issue_is_reported_for(@"
public class TestMe
{
   public void DoSomething()
   {
   }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_usage_in_a_test_method([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething()
        {
            Assert.That(42, Is.Not.EqualTo(0815));
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_correct_usage_in_a_non_test_method_inside_a_test([ValueSource(nameof(TestFixtures))] string testFixture) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [" + testFixture + @"]
    public class TestMe
    {
        public void DoSomething()
        {
            Assert.That(42, Is.Not.EqualTo(0815));
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_correct_usage_in_a_non_test_class() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            Assert.That(42, Is.Not.EqualTo(0815));
        }
    }
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_an_operator_in_a_test_method([ValueSource(nameof(Tests))] string test, [ValueSource(nameof(Operators))] string @operator) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething()
        {
            Assert.IsTrue(42 " + @operator + @" 0815);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_an_operator_in_a_non_test_method_inside_a_test([ValueSource(nameof(TestFixtures))] string testFixture, [ValueSource(nameof(Operators))] string @operator) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [" + testFixture + @"]
    public class TestMe
    {
        public void DoSomething()
        {
            Assert.IsTrue(42 " + @operator + @" 0815);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_an_operator_in_a_non_test_class([ValueSource(nameof(Operators))] string @operator) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            Assert.IsTrue(42 " + @operator + @" 0815);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_an_operator_in_an_Assert_Multiple([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            Assert.Multiple(() => {
                                    var x = 4711;
                                    var y = 0815;
                                    var result = x " + @operator + @" y;
                                  });
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_boolean_operation_in_a_non_test_class([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            Assert.IsTrue(42." + method + @"(0815));
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3106_TestAssertsDoNotUseOperatorsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3106_TestAssertsDoNotUseOperatorsAnalyzer();
    }
}