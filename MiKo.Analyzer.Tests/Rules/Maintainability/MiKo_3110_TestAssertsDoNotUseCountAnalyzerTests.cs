using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3110_TestAssertsDoNotUseCountAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AssertionMethods =
            {
                nameof(Assert.AreEqual),
                nameof(Assert.AreNotEqual),
                nameof(Assert.AreSame),
                nameof(Assert.AreNotSame),
                nameof(Assert.Less),
                nameof(Assert.LessOrEqual),
                nameof(Assert.Greater),
                nameof(Assert.GreaterOrEqual),
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
            Assert." + assertion + "(values." + culprit + @", Is.EqualTo(42));
        }
    }
}
");
                                                                                                              }
                                                                                                          }
                                                                                                      });

        [TestCase("Assert.That(values.Length, Is.EqualTo(42))", "Assert.That(values, Has.Count.EqualTo(42))")]
        [TestCase("Assert.That(values.Count, Is.EqualTo(42))", "Assert.That(values, Has.Count.EqualTo(42))")]
        [TestCase("Assert.AreEqual(42, values.Length)", "Assert.That(values, Has.Count.EqualTo(42))")]
        [TestCase("Assert.AreEqual(42, values.Count)", "Assert.That(values, Has.Count.EqualTo(42))")]
        [TestCase("Assert.AreEqual(values.Length, 42)", "Assert.That(values, Has.Count.EqualTo(42))")]
        [TestCase("Assert.AreNotEqual(42, values.Length)", "Assert.That(values, Has.Count.Not.EqualTo(42))")]
        [TestCase("Assert.AreNotEqual(42, values.Count)", "Assert.That(values, Has.Count.Not.EqualTo(42))")]
        [TestCase("Assert.AreNotEqual(values.Length, 42)", "Assert.That(values, Has.Count.Not.EqualTo(42))")]
        [TestCase("Assert.Less(values.Length, 42)", "Assert.That(values, Has.Count.LessThan(42))")]
        [TestCase("Assert.LessOrEqual(values.Length, 42)", "Assert.That(values, Has.Count.LessThanOrEqualTo(42))")]
        [TestCase("Assert.Greater(values.Length, 42)", "Assert.That(values, Has.Count.GreaterThan(42))")]
        [TestCase("Assert.GreaterOrEqual(values.Length, 42)", "Assert.That(values, Has.Count.GreaterThanOrEqualTo(42))")]
        public void Code_gets_fixed_for_(string originalCode, string fixedCode)
        {
            const string Template = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(int[] values)
        {
            ###;
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [TestCase("Assert.That(values.Count(), Is.EqualTo(42))", "Assert.That(values, Has.Count.EqualTo(42))")]
        [TestCase("Assert.AreEqual(42, values.Count())", "Assert.That(values, Has.Count.EqualTo(42))")]
        [TestCase("Assert.AreEqual(values.Count(), 42)", "Assert.That(values, Has.Count.EqualTo(42))")]
        [TestCase("Assert.AreNotEqual(42, values.Count())", "Assert.That(values, Has.Count.Not.EqualTo(42))")]
        [TestCase("Assert.AreNotEqual(values.Count(), 42)", "Assert.That(values, Has.Count.Not.EqualTo(42))")]
        public void Code_gets_fixed_for_Linq_call_(string originalCode, string fixedCode)
        {
            const string OriginalTemplate = @"
using System;
using System.Linq;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(int[] values)
        {
            ###;
        }

        // just another test to avoid compiler warning because of unused 'System.Linq'
        [Test]
        public void DoSomethingElse(int[] values)
        {
            Assert.That(values.SkipWhile(_ => true), Is.Empty);
        }
    }
}";

            const string FixedTemplate = @"
using System;
using System.Linq;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(int[] values)
        {
            ###;
        }

        // just another test to avoid compiler warning because of unused 'System.Linq'
        [Test]
        public void DoSomethingElse(int[] values)
        {
            Assert.That(values.SkipWhile(_ => true), Is.Empty);
        }
    }
}";

            VerifyCSharpFix(OriginalTemplate.Replace("###", originalCode), FixedTemplate.Replace("###", fixedCode));
        }

        [Test]
        public void Code_gets_fixed_for_List_on_ObjectUnderTest()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public int[] Values { get; set; }
    }

    public class TestMeTests
    {
        [Test]
        public void DoSomething()
        {
            var objectUnderTest = new TestMe();

            Assert.AreEqual(42, objectUnderTest.Values.Count);
        }
    }
}";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public int[] Values { get; set; }
    }

    public class TestMeTests
    {
        [Test]
        public void DoSomething()
        {
            var objectUnderTest = new TestMe();

            Assert.That(objectUnderTest.Values, Has.Count.EqualTo(42));
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_3110_TestAssertsDoNotUseCountAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3110_TestAssertsDoNotUseCountAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3110_CodeFixProvider();
    }
}