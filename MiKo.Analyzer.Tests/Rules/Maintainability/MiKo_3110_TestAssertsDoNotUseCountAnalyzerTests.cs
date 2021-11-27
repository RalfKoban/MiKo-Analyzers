using Microsoft.CodeAnalysis.Diagnostics;

using NCrunch.Framework;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture, Isolated]
    public sealed class MiKo_3110_TestAssertsDoNotUseCountAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AssertionMethods =
            {
                nameof(Assert.AreEqual),
                nameof(Assert.AreNotEqual),
                nameof(Assert.AreSame),
                nameof(Assert.AreNotSame),
            };

        private static readonly string[] Culprits =
            {
                "Length",
                "Count",
                "Count()",
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
        public void No_issue_is_reported_for_correct_usage_in_a_test_method_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething(int[] values)
        {
            Assert.That(values, Has.Count.EqualTo(42));
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_Length_in_an_Assert_That_test_method_([ValueSource(nameof(Culprits))] string culprit) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(int[] values)
        {
            Assert.That(values." + culprit + @", Is.EqualTo(42));
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Length_in_an_Assert_test_method() => Assert.Multiple(() =>
                                                                                                      {
                                                                                                          foreach (var culprit in Culprits)
                                                                                                          {
                                                                                                              foreach (var assertion in AssertionMethods)
                                                                                                              {
                                                                                                                  An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(int[] values)
        {
            Assert." + assertion + @"(values." + culprit + @", Is.EqualTo(42));
        }
    }
}
");
                                                                                                              }
                                                                                                          }
                                                                                                      });

        protected override string GetDiagnosticId() => MiKo_3110_TestAssertsDoNotUseCountAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3110_TestAssertsDoNotUseCountAnalyzer();
    }
}