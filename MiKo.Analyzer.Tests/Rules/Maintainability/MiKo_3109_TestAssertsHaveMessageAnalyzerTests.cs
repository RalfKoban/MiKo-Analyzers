using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3109_TestAssertsHaveMessageAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AssertionsWithoutMessages =
            {
                "Assert.That(42, Is.Not.EqualTo(0815))",
                "Assert.AreEqual(42, 0815)",
                "Assert.IsNull(null)",
                "Assert.False(true)",
                "Assert.Fail()",
                "Assert.Pass()",
                "Assert.AreSame(1, 2)",
            };

        private static readonly string[] AssertionsWithMessages =
            {
                @"Assert.That(42, Is.Not.EqualTo(0815), ""some message {0}"", 42)",
                @"Assert.That(42, Is.Not.EqualTo(0815), ""some message "" + 42)",
                @"Assert.That(42, Is.Not.EqualTo(0815), ""some message"")",
                @"Assert.That(42, Is.Not.EqualTo(0815), 42 + "" some message "")",
                @"Assert.That(42, Is.Not.EqualTo(0815), $""{42} some message "")",
                @"Assert.That(42, Is.Not.EqualTo(0815), $""some message {42} "")",
                @"Assert.That(42, Is.Not.EqualTo(0815), ""some message "" + 42 + ""some more message"")",
                @"Assert.That(42, Is.Not.EqualTo(0815), ""some message "" + 42 + ""some more message"" + 0815)",
                @"Assert.AreEqual(42, 0815, ""some message"")",
                @"Assert.IsNull(null, ""some message {0}"", 42)",
                @"Assert.IsNull(null, ""some message"")",
                @"Assert.False(true, ""some message"")",
                @"Assert.Fail(""some message {0}"", 42)",
                @"Assert.Fail(""some message"")",
                @"Assert.Pass(""some message"")",
                @"Assert.AreSame(1, 2, ""some message"")",
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
        public void No_issue_is_reported_for_empty_test_method() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_non_empty_test_method_without_assertion() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            var x = 0;
            x = x + 1;

            var y = x.ToString();
            System.Console.Write(y);
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_uses_an_assertion_with_message_([ValueSource(nameof(AssertionsWithMessages))] string assertion) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            " + assertion + @";

            // do it again to have at least 2 assertions inside the method
            " + assertion + @";
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_uses_a_single_assertion_with_no_message_([ValueSource(nameof(AssertionsWithoutMessages))] string assertion) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            " + assertion + @";
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_uses_a_parameter_as_assertion_message_() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething(string message)
        {
            Assert.That(42, Is.EqualTo(42), message);
            Assert.That(-1, Is.Negative, message);
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_test_method_that_uses_more_than_1_assertion_with_no_message_([ValueSource(nameof(AssertionsWithoutMessages))] string assertion) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            Assert.Fail(""fix me"");

            " + assertion + @";
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_uses_AssertMultiple_and_1_assertion_with_no_message_([ValueSource(nameof(AssertionsWithoutMessages))] string assertion) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            Assert.Multiple(() =>
                            {
                                " + assertion + @";
                            });
        }
    }
}");

        [Test]
        public void Exactly_2_issues_are_reported_for_a_test_method_that_uses_AssertMultiple_and_2_assertions_with_no_messages_([ValueSource(nameof(AssertionsWithoutMessages))] string assertion) => An_issue_is_reported_for(
@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            Assert.Multiple(() =>
                            {
                                " + assertion + @";
                                " + assertion + @";
                            });
        }
    }
}",
2); // 1 issue for each assertion

        [TestCase("Assert.That(values.Length, Is.EqualTo(42))", @"Assert.That(values.Length, Is.EqualTo(42), ""wrong length"")")]
        [TestCase("Assert.That(values.GetHashCode(), Is.EqualTo(42))", @"Assert.That(values.GetHashCode(), Is.EqualTo(42), ""wrong hash code"")")]
        [TestCase("Assert.That(values.GetId(), Is.EqualTo(42))", @"Assert.That(values.GetId(), Is.EqualTo(42), ""wrong identifier"")")]
        [TestCase("Assert.That(values.HasId(), Is.True)", @"Assert.That(values.HasId(), Is.True, ""wrong identifier"")")]
        [TestCase("Assert.That(values.IsId(), Is.True)", @"Assert.That(values.IsId(), Is.True, ""wrong identifier"")")]
        [TestCase("Assert.That(values.CanSave(), Is.True)", @"Assert.That(values.CanSave(), Is.True, ""wrong save"")")]
        [TestCase("Assert.That(values.Contains(42), Is.True)", @"Assert.That(values.Contains(42), Is.True, ""wrong value"")")]
        [TestCase("Assert.That(values, Is.Not.Null)", @"Assert.That(values, Is.Not.Null, ""missing values"")")]
        [TestCase("Assert.That(Guid.Parse(values), Is.EqualTo(42))", @"Assert.That(Guid.Parse(values), Is.EqualTo(42), ""wrong values"")")]
        [TestCase("Assert.That(new Guid(values), Is.EqualTo(42))", @"Assert.That(new Guid(values), Is.EqualTo(42), ""wrong guid"")")]
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
            Assert.Fail(""some error reason"");
        }
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        protected override string GetDiagnosticId() => MiKo_3109_TestAssertsHaveMessageAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3109_TestAssertsHaveMessageAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3109_CodeFixProvider();
    }
}