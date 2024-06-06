using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3110_TestAssertsDoNotUseCountAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AssertionMethods =
                                                            {
                                                                "AreEqual",
                                                                "AreNotEqual",
                                                                "AreSame",
                                                                "AreNotSame",
                                                                "Less",
                                                                "LessOrEqual",
                                                                "Greater",
                                                                "GreaterOrEqual",
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
        public void No_issue_is_reported_for_HasCount_usage_in_a_test_method_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_HasExactlyItems_usage_in_a_test_method_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething(int[] values)
        {
            Assert.That(values, Has.Exactly(42).Items);
        }
    }
}");

        [TestCase("Assert.That(values.Count(), Is.Not.EqualTo(42))")]
        [TestCase("Assert.AreNotEqual(42, values.Count())")]
        [TestCase("Assert.AreNotEqual(values.Count(), 42)")]
        [TestCase("Assert.That(values.Count(), Is.GreaterThan(42))")]
        public void No_issue_is_reported_for_Linq_call_(string assertion) => No_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(IEnumerable<int> values)
        {
            " + assertion + @";
        }
    }
}
");

        [TestCase("Assert.AreEqual(42, values.Count())")]
        [TestCase("Assert.AreEqual(values.Count(), 42)")]
        [TestCase("Assert.That(values.Count(), Is.EqualTo(42))")]
        [TestCase("Assert.That(values.Count(), Is.EqualTo(0))")]
        [TestCase("Assert.That(values.Count(), Is.Zero)")]
        public void An_issue_is_reported_for_Linq_call_(string assertion) => An_issue_is_reported_for(@"
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(IEnumerable<int> values)
        {
            " + assertion + @";
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Count_property_in_an_Assert_That_test_method() => An_issue_is_reported_for(@"
using System.Collections.Generic;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(List<int> values)
        {
            Assert.That(values.Count, Is.EqualTo(42));
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Length_property_in_an_Assert_That_test_method() => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(int[] values)
        {
            Assert.That(values.Length, Is.EqualTo(42));
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Count_property_in_an_Assert_test_method() => Assert.Multiple(() =>
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
        public void DoSomething(List<int> values)
        {
            Assert." + assertion + @"(values.Count, Is.EqualTo(42));
        }
    }
}
");
                                                                                                                   }
                                                                                                               });

        [Test]
        public void An_issue_is_reported_for_Length_property_in_an_Assert_test_method() => Assert.Multiple(() =>
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
            Assert." + assertion + @"(values.Length, Is.EqualTo(42));
        }
    }
}
");
                                                                                                                    }
                                                                                                                });

        [TestCase("Assert.That(values.Count, Is.Zero)", "Assert.That(values, Is.Empty)")]
        [TestCase("Assert.That(values.Count, Is.EqualTo(0))", "Assert.That(values, Is.Empty)")]
        [TestCase("Assert.That(values.Count, Is.EqualTo(42))", "Assert.That(values, Has.Exactly(42).Items)")]
        [TestCase("Assert.That(values.Count, Is.EqualTo(Int16.MaxValue))", "Assert.That(values, Has.Exactly(Int16.MaxValue).Items)")]
        [TestCase("Assert.That(values.Count, Is.EqualTo(Random.Next()))", "Assert.That(values, Has.Exactly(Random.Next()).Items)")]
        [TestCase("Assert.AreEqual(42, values.Count)", "Assert.That(values, Has.Exactly(42).Items)")]
        [TestCase("Assert.AreEqual(values.Count, 42)", "Assert.That(values, Has.Exactly(42).Items)")]
        [TestCase("Assert.AreNotEqual(42, values.Count)", "Assert.That(values, Has.Count.Not.EqualTo(42))")]
        [TestCase("Assert.AreNotEqual(values.Count, 42)", "Assert.That(values, Has.Count.Not.EqualTo(42))")]
        [TestCase("Assert.Less(values.Count, 42)", "Assert.That(values, Has.Count.LessThan(42))")]
        [TestCase("Assert.LessOrEqual(values.Count, 42)", "Assert.That(values, Has.Count.LessThanOrEqualTo(42))")]
        [TestCase("Assert.Greater(values.Count, 42)", "Assert.That(values, Has.Count.GreaterThan(42))")]
        [TestCase("Assert.GreaterOrEqual(values.Count, 42)", "Assert.That(values, Has.Count.GreaterThanOrEqualTo(42))")]
        public void Code_gets_fixed_for_Count_(string originalCode, string fixedCode)
        {
            const string Template = @"
using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(List<int> values)
        {
            ###;
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [TestCase("Assert.That(values.Length, Is.Zero)", "Assert.That(values, Is.Empty)")]
        [TestCase("Assert.That(values.Length, Is.EqualTo(0))", "Assert.That(values, Is.Empty)")]
        [TestCase("Assert.That(values.Length, Is.EqualTo(42))", "Assert.That(values, Has.Exactly(42).Items)")]
        [TestCase("Assert.That(values.Length, Is.EqualTo(Int16.MaxValue))", "Assert.That(values, Has.Exactly(Int16.MaxValue).Items)")]
        [TestCase("Assert.That(values.Length, Is.EqualTo(Random.Next()))", "Assert.That(values, Has.Exactly(Random.Next()).Items)")]
        [TestCase("Assert.AreEqual(42, values.Length)", "Assert.That(values, Has.Exactly(42).Items)")]
        [TestCase("Assert.AreEqual(values.Length, 42)", "Assert.That(values, Has.Exactly(42).Items)")]
        [TestCase("Assert.AreNotEqual(42, values.Length)", "Assert.That(values, Has.Length.Not.EqualTo(42))")]
        [TestCase("Assert.AreNotEqual(values.Length, 42)", "Assert.That(values, Has.Length.Not.EqualTo(42))")]
        [TestCase("Assert.Less(values.Length, 42)", "Assert.That(values, Has.Length.LessThan(42))")]
        [TestCase("Assert.LessOrEqual(values.Length, 42)", "Assert.That(values, Has.Length.LessThanOrEqualTo(42))")]
        [TestCase("Assert.Greater(values.Length, 42)", "Assert.That(values, Has.Length.GreaterThan(42))")]
        [TestCase("Assert.GreaterOrEqual(values.Length, 42)", "Assert.That(values, Has.Length.GreaterThanOrEqualTo(42))")]
        public void Code_gets_fixed_for_Length_(string originalCode, string fixedCode)
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

        [TestCase("Assert.AreEqual(42, values.Count())", "Assert.That(values, Has.Exactly(42).Items)")]
        [TestCase("Assert.AreEqual(values.Count(), 42)", "Assert.That(values, Has.Exactly(42).Items)")]
        [TestCase("Assert.That(values.Count(), Is.Zero)", "Assert.That(values, Is.Empty)")]
        [TestCase("Assert.That(values.Count(), Is.EqualTo(0))", "Assert.That(values, Is.Empty)")]
        [TestCase("Assert.That(values.Count(), Is.EqualTo(42))", "Assert.That(values, Has.Exactly(42).Items)")]
        public void Code_gets_fixed_for_Linq_call_(string originalCode, string fixedCode)
        {
            const string OriginalTemplate = @"
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

            Assert.That(objectUnderTest.Values, Has.Exactly(42).Items);
        }
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_local_function_call()
        {
            const string OriginalCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMeTests
    {
        [Test]
        public void DoSomething(int[] values)
        {
            int Factorial(int number) => number <= 1 ? 1 : number * Factorial(number - 1);

            Assert.That(values.Length, Is.EqualTo(Factorial(42)));
        }
    }
}";

            const string FixedCode = @"
using System;

using NUnit.Framework;

namespace Bla
{
    public class TestMeTests
    {
        [Test]
        public void DoSomething(int[] values)
        {
            int Factorial(int number) => number <= 1 ? 1 : number * Factorial(number - 1);

            Assert.That(values, Has.Exactly(Factorial(42)).Items);
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