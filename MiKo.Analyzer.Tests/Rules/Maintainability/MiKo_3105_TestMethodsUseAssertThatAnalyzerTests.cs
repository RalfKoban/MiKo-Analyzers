using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3105_TestMethodsUseAssertThatAnalyzerTests : CodeFixVerifier
    {
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
        public void No_issue_is_reported_for_preferred_usage_in_a_test_method_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_preferred_usage_in_a_non_test_method_inside_a_test_([ValueSource(nameof(TestFixtures))] string testFixture) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_preferred_usage_in_a_non_test_class() => No_issue_is_reported_for(@"
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

        [Test]
        public void An_issue_is_reported_for_a_test_method_([ValueSource(nameof(Tests))] string test) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething()
        {
            Assert.AreEqual(42, 0815);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_non_test_method_inside_a_test_([ValueSource(nameof(TestFixtures))] string testFixture) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [" + testFixture + @"]
    public class TestMe
    {
        public void DoSomething()
        {
            Assert.AreEqual(42, 0815);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_non_test_class() => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            Assert.AreEqual(42, 0815);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_a_AssertMultiple() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething()
        {
            Assert.Multiple(() => { });
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_in_nameof() => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        private static readonly HashSet<string> Names = new HashSet<string>
                                                            {
                                                                nameof(Assert.AreEqual),
                                                            };
            
        public void DoSomething()
        {
            Names.Clear();
        }
    }
}
");

        // IsNull
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNull(o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]

        // Null
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.Null(o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.Null(o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null, ""my message""); }")]

        // NotNull
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.NotNull(o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.NotNull(o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null, ""my message""); }")]

        // IsNotEmpty
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNotEmpty(o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Empty); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNotEmpty(o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Empty, ""my message""); }")]

        // IsTrue
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.IsTrue(b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.IsTrue(b, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True, ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.IsTrue(a == b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.That(a, Is.EqualTo(b)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.IsTrue(a != b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.That(a, Is.Not.EqualTo(b)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsTrue(o == null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsTrue(o != null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsTrue(null == o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsTrue(null != o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsTrue(i == 1); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.EqualTo(1)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsTrue(i < 5); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.LessThan(5)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsTrue(i <= 5); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.LessThanOrEqualTo(5)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsTrue(i > 5); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.GreaterThan(5)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsTrue(i >= 5); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.GreaterThanOrEqualTo(5)); }")]

        // True
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.True(b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]

        // IsFalse
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.IsFalse(b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.IsFalse(b, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False, ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.IsFalse(a == b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.That(a, Is.Not.EqualTo(b)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.IsFalse(a != b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.That(a, Is.EqualTo(b)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsFalse(o == null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsFalse(o != null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsFalse(null == o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsFalse(null != o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsFalse(i == 1); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.Not.EqualTo(1)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsFalse(i < 5); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.GreaterThanOrEqualTo(5)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsFalse(i <= 5); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.GreaterThan(5)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsFalse(i > 5); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.LessThanOrEqualTo(5)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsFalse(i >= 5); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.LessThan(5)); }")]
        [TestCase(
             @"using System; using System.IO; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.IsFalse(File.Exists(""a.txt""); }",
             @"using System; using System.IO; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(File.Exists(""a.txt""), Is.False); }")]

        // False
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.False(b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]

        // AreEqual
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.AreEqual(42, 11); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(11, Is.EqualTo(42)); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.AreEqual(42, 11, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(11, Is.EqualTo(42), ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreEqual(true, b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreEqual(b, true); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreEqual(false, b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreEqual(b, false); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.AreEqual(i, 42); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.EqualTo(42)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.AreEqual(42, i); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.EqualTo(42)); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.AreEqual(s, ""abc""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Is.EqualTo(""abc"")); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.AreEqual(""abc"", s); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Is.EqualTo(""abc"")); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreEqual(null, o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreEqual(o, null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.AreEqual(8.5d, d, double.Epsilon); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.That(d, Is.EqualTo(8.5d).Within(double.Epsilon)); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.AreEqual(8.5d, d, double.Epsilon, ""some message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.That(d, Is.EqualTo(8.5d).Within(double.Epsilon), ""some message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.AreEqual(8.5d, d, 0.1d, ""some message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.That(d, Is.EqualTo(8.5d).Within(0.1d), ""some message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.AreEqual(d, 8.5d, double.Epsilon, ""some message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.That(d, Is.EqualTo(8.5d).Within(double.Epsilon), ""some message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.AreEqual(d, 8.5d, 0.1d, ""some message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.That(d, Is.EqualTo(8.5d).Within(0.1d), ""some message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(GCNotificationStatus status) => Assert.AreEqual(status, GCNotificationStatus.Failed); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(GCNotificationStatus status) => Assert.That(status, Is.EqualTo(GCNotificationStatus.Failed)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(GCNotificationStatus status) => Assert.AreEqual(GCNotificationStatus.Failed, status); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(GCNotificationStatus status) => Assert.That(status, Is.EqualTo(GCNotificationStatus.Failed)); }")]

        // AreEqual (expected & actual variables)
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int expected) => Assert.AreEqual(expected, actual); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int expected) => Assert.That(actual, Is.EqualTo(expected)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int expected) => Assert.AreEqual(actual, expected); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int expected) => Assert.That(actual, Is.EqualTo(expected)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int expectedValue) => Assert.AreEqual(expectedValue, actualValue); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int expectedValue) => Assert.That(actualValue, Is.EqualTo(expectedValue)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int expectedValue) => Assert.AreEqual(actualValue, expectedValue); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int expectedValue) => Assert.That(actualValue, Is.EqualTo(expectedValue)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int other) => Assert.AreEqual(other, actual); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int other) => Assert.That(actual, Is.EqualTo(other)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int other) => Assert.AreEqual(actual, other); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int other) => Assert.That(actual, Is.EqualTo(other)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int otherValue) => Assert.AreEqual(otherValue, actualValue); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int otherValue) => Assert.That(actualValue, Is.EqualTo(otherValue)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int otherValue) => Assert.AreEqual(actualValue, otherValue); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int otherValue) => Assert.That(actualValue, Is.EqualTo(otherValue)); }")]

        // AreNotEqual
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.AreNotEqual(42, 11); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(11, Is.Not.EqualTo(42)); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.AreNotEqual(42, 11, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(11, Is.Not.EqualTo(42), ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreNotEqual(true, b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreNotEqual(b, true); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreNotEqual(false, b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreNotEqual(b, false); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreNotEqual(null, o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreNotEqual(o, null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.AreNotEqual(i, 42); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.Not.EqualTo(42)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.AreNotEqual(42, i); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.Not.EqualTo(42)); }")]

        // AreNotEqual (expected & actual variables)
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int expected) => Assert.AreNotEqual(expected, actual); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int expected) => Assert.That(actual, Is.Not.EqualTo(expected)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int expected) => Assert.AreNotEqual(actual, expected); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int expected) => Assert.That(actual, Is.Not.EqualTo(expected)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int expectedValue) => Assert.AreNotEqual(expectedValue, actualValue); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int expectedValue) => Assert.That(actualValue, Is.Not.EqualTo(expectedValue)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int expectedValue) => Assert.AreNotEqual(actualValue, expectedValue); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int expectedValue) => Assert.That(actualValue, Is.Not.EqualTo(expectedValue)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int other) => Assert.AreNotEqual(other, actual); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int other) => Assert.That(actual, Is.Not.EqualTo(other)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int other) => Assert.AreNotEqual(actual, other); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actual, int other) => Assert.That(actual, Is.Not.EqualTo(other)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int otherValue) => Assert.AreNotEqual(otherValue, actualValue); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int otherValue) => Assert.That(actualValue, Is.Not.EqualTo(otherValue)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int otherValue) => Assert.AreNotEqual(actualValue, otherValue); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int actualValue, int otherValue) => Assert.That(actualValue, Is.Not.EqualTo(otherValue)); }")]

        // Greater, GreaterOrEqual, Less, LessOrEqual
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Greater(42, 11); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(42, Is.GreaterThan(11)); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Greater(42, 11, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(42, Is.GreaterThan(11), ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.GreaterOrEqual(42, 11); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(42, Is.GreaterThanOrEqualTo(11)); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.GreaterOrEqual(42, 11, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(42, Is.GreaterThanOrEqualTo(11), ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Less(42, 11); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(42, Is.LessThan(11)); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Less(42, 11, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(42, Is.LessThan(11), ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.LessOrEqual(42, 11); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(42, Is.LessThanOrEqualTo(11)); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.LessOrEqual(42, 11, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(42, Is.LessThanOrEqualTo(11), ""my message""); }")]

        // AreSame
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreSame(null, o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreSame(o, null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreSame(false, b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreSame(b, false); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreSame(b, true); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreSame(true, b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.AreSame(42, i); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.SameAs(42)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.AreSame(i, 42); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.SameAs(42)); }")]

        // AreNotSame
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreNotSame(null, o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreNotSame(o, null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreNotSame(false, b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreNotSame(b, false); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreNotSame(b, true); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.AreNotSame(true, b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.AreNotSame(42, i); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.Not.SameAs(42)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.AreNotSame(i, 42); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.Not.SameAs(42)); }")]

        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNullOrEmpty(o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null.Or.Empty, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNotNull(o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.IsEmpty(s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Is.Empty, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e, IEnumerable a) => CollectionAssert.AreEquivalent(e, a, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e, IEnumerable a) => Assert.That(a, Is.EquivalentTo(e), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e, IEnumerable a) => CollectionAssert.AreNotEquivalent(e, a, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e, IEnumerable a) => Assert.That(a, Is.Not.EquivalentTo(e), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e, object o) => CollectionAssert.DoesNotContain(e, o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e, object o) => Assert.That(e, Does.Not.Contain(o), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e, object o) => CollectionAssert.Contains(e, o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e, object o) => Assert.That(e, Does.Contain(o), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => StringAssert.StartsWith(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Does.StartWith(""abc""), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => StringAssert.EndsWith(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Does.EndWith(""abc""), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => StringAssert.DoesNotContain(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Does.Not.Contain(""abc""), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => StringAssert.DoesNotStartWith(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Does.Not.StartWith(""abc""), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => StringAssert.DoesNotEndWith(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Does.Not.EndWith(""abc""), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => StringAssert.Contains(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Does.Contain(""abc""), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => StringAssert.AreEqualIgnoringCase(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Is.EqualTo(""abc"").IgnoreCase, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => StringAssert.AreNotEqualIgnoringCase(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Is.Not.EqualTo(""abc"").IgnoreCase, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => CollectionAssert.IsSubsetOf(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Is.SubsetOf(""abc""), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => CollectionAssert.IsNotSubsetOf(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Is.Not.SubsetOf(""abc""), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => CollectionAssert.IsSupersetOf(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Is.SupersetOf(""abc""), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => CollectionAssert.IsNotSupersetOf(""abc"", s, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(string s) => Assert.That(s, Is.Not.SupersetOf(""abc""), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsInstanceOf(typeof(object), o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.InstanceOf<object>(), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsInstanceOf<object>(o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.InstanceOf<object>(), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNotInstanceOf(typeof(object), o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.InstanceOf<object>(), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNotInstanceOf<object>(o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.InstanceOf<object>(), ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => CollectionAssert.IsOrdered(e, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => Assert.That(e, Is.Ordered, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => CollectionAssert.AllItemsAreNotNull(e, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => Assert.That(e, Is.All.Not.Null, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => CollectionAssert.AllItemsAreUnique(e, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => Assert.That(e, Is.Unique, ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.Zero(i); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.Zero); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.NotZero(i); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.Not.Zero); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.IsNaN(d); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(double d) => Assert.That(d, Is.NaN); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.Negative(i); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.Negative); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.Positive(i); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.Positive); }")]

        // IsAssignableFrom
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.IsAssignableFrom(typeof(object), new object()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(new object(), Is.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.IsAssignableFrom<object>(new object()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(new object(), Is.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.IsNotAssignableFrom(typeof(object), new object()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(new object(), Is.Not.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.IsNotAssignableFrom<object>(new object()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(new object(), Is.Not.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.IsTrue(type.IsAssignableFrom(typeof(object)); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.That(type, Is.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.IsTrue(type.IsAssignableFrom<object>()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.That(type, Is.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.IsTrue(type.IsNotAssignableFrom(typeof(object)); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.That(type, Is.Not.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.IsTrue(type.IsNotAssignableFrom<object>()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.That(type, Is.Not.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.IsFalse(type.IsAssignableFrom(typeof(object)); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.That(type, Is.Not.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.IsFalse(type.IsAssignableFrom<object>()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.That(type, Is.Not.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.IsFalse(type.IsNotAssignableFrom(typeof(object)); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.That(type, Is.AssignableFrom<object>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.IsFalse(type.IsNotAssignableFrom<object>()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(Type type) => Assert.That(type, Is.AssignableFrom<object>()); }")]

        [TestCase(
             @"using System; using System.Text.RegularExpressions; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => StringAssert.IsMatch(""some pattern"", ""actual""); }",
             @"using System; using System.Text.RegularExpressions; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(""actual"", Does.Match(""some pattern"")); }")]
        [TestCase(
             @"using System; using System.Text.RegularExpressions; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => StringAssert.DoesNotMatch(""some pattern"", ""actual""); }",
             @"using System; using System.Text.RegularExpressions; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(""actual"", Does.Not.Match(""some pattern"")); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable collection) => CollectionAssert.AllItemsAreInstancesOfType(collection, typeof(string)); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable collection) => Assert.That(collection, Is.All.InstanceOf<string>()); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.DoesNotThrow(() => throw new ArgumentException()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new ArgumentException(), Throws.Nothing); }")]
        [TestCase(
             "using System; using System.Threading.Tasks; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.DoesNotThrowAsync(async () => await Task.CompletedTask); }",
             "using System; using System.Threading.Tasks; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(async () => await Task.CompletedTask, Throws.Nothing); }")]

        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => FileAssert.Exists(@""c:\pagefile.sys""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(@""c:\pagefile.sys"", Does.Exist); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => DirectoryAssert.Exists(@""c:\Windows""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(@""c:\Windows"", Does.Exist); }")]

        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => FileAssert.DoesNotExist(@""c:\pagefile.sys""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(@""c:\pagefile.sys"", Does.Not.Exist); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => DirectoryAssert.DoesNotExist(@""c:\Windows""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(@""c:\Windows"", Does.Not.Exist); }")]

        // Throws
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws<ArgumentNullException>(() => throw new Exception()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.ArgumentNullException); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws<ArgumentNullException>(() => throw new Exception(), ""some message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.ArgumentNullException, ""some message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws<ArgumentException>(() => throw new Exception()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.ArgumentException); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws<ArgumentException>(() => throw new Exception(), ""some message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.ArgumentException, ""some message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws<InvalidOperationException>(() => throw new Exception()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.InvalidOperationException); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws<InvalidOperationException>(() => throw new Exception(), ""some message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.InvalidOperationException, ""some message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws<TargetInvocationException>(() => throw new Exception()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.TargetInvocationException); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws<TargetInvocationException>(() => throw new Exception(), ""some message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.TargetInvocationException, ""some message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws<NotSupportedException>(() => throw new Exception()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.TypeOf<NotSupportedException>()); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws<NotSupportedException>(() => throw new Exception(), ""some message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.TypeOf<NotSupportedException>(), ""some message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws(typeof(ApplicationException), () => throw new Exception()); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.TypeOf<ApplicationException>()); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.Throws(typeof(ApplicationException), () => throw new Exception(), ""some message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(() => throw new Exception(), Throws.TypeOf<ApplicationException>(), ""some message""); }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        [Test]
        public void Code_gets_fixed_for_Assert_with_comment()
        {
            const string OriginalCode = @"
using System;
using NUnit.Framework;

[TestFixture]
class TestMe
{
    [Test]
    void Do()
    {
        var s = string.Empty;

        // some comment
        Assert.AreEqual(""my message"", s);
    }
}";

            const string FixedCode = @"
using System;
using NUnit.Framework;

[TestFixture]
class TestMe
{
    [Test]
    void Do()
    {
        var s = string.Empty;

        // some comment
        Assert.That(s, Is.EqualTo(""my message""));
    }
}";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [TestCase("Assert.AreEqual(GCNotificationStatus.Failed, ObjectUnderTest.Status)", "Assert.That(ObjectUnderTest.Status, Is.EqualTo(GCNotificationStatus.Failed))")]
        [TestCase("Assert.AreEqual(ObjectUnderTest.Status, GCNotificationStatus.Failed)", "Assert.That(ObjectUnderTest.Status, Is.EqualTo(GCNotificationStatus.Failed))")]
        [TestCase("Assert.AreNotEqual(GCNotificationStatus.Failed, ObjectUnderTest.Status)", "Assert.That(ObjectUnderTest.Status, Is.Not.EqualTo(GCNotificationStatus.Failed))")]
        [TestCase("Assert.AreNotEqual(ObjectUnderTest.Status, GCNotificationStatus.Failed)", "Assert.That(ObjectUnderTest.Status, Is.Not.EqualTo(GCNotificationStatus.Failed))")]
        public void Code_gets_fixed_for_Assert_on_object_under_test_(string originalCode, string fixedCode)
        {
            const string Template = @"
using System;
using NUnit.Framework;

public interface ITestee
{
    GCNotificationStatus Status { get; }
}

[TestFixture]
public class TestMe
{
    private ITestee ObjectUnderTest { get; set; }

    [Test]
    void Do()
    {
        ###;
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [TestCase("Assert.AreEqual(GCNotificationStatus.Failed, result)", "Assert.That(result, Is.EqualTo(GCNotificationStatus.Failed))")]
        [TestCase("Assert.AreEqual(result, GCNotificationStatus.Failed)", "Assert.That(result, Is.EqualTo(GCNotificationStatus.Failed))")]
        [TestCase("Assert.AreNotEqual(GCNotificationStatus.Failed, result)", "Assert.That(result, Is.Not.EqualTo(GCNotificationStatus.Failed))")]
        [TestCase("Assert.AreNotEqual(result, GCNotificationStatus.Failed)", "Assert.That(result, Is.Not.EqualTo(GCNotificationStatus.Failed))")]
        public void Code_gets_fixed_for_Assert_on_object_under_test_return_value_(string originalCode, string fixedCode)
        {
            const string Template = @"
using System;
using NUnit.Framework;

public interface ITestee
{
    GCNotificationStatus DoSomething();
}

[TestFixture]
public class TestMe
{
    private ITestee ObjectUnderTest { get; set; }

    [Test]
    void Do()
    {
        var result = ObjectUnderTest.DoSomething();

        ###;
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [TestCase("Assert.AreEqual(GCNotificationStatus.Failed, result.Value)", "Assert.That(result.Value, Is.EqualTo(GCNotificationStatus.Failed))")]
        [TestCase("Assert.AreEqual(result.Value, GCNotificationStatus.Failed)", "Assert.That(result.Value, Is.EqualTo(GCNotificationStatus.Failed))")]
        [TestCase("Assert.AreNotEqual(GCNotificationStatus.Failed, result.Value)", "Assert.That(result.Value, Is.Not.EqualTo(GCNotificationStatus.Failed))")]
        [TestCase("Assert.AreNotEqual(result.Value, GCNotificationStatus.Failed)", "Assert.That(result.Value, Is.Not.EqualTo(GCNotificationStatus.Failed))")]
        public void Code_gets_fixed_for_Assert_on_object_under_test_returned_property_value_(string originalCode, string fixedCode)
        {
            const string Template = @"
using System;
using NUnit.Framework;

public interface ITestee
{
    ITestee2 DoSomething();
}

public interface ITestee2
{
    GCNotificationStatus Value;
}

[TestFixture]
public class TestMe
{
    private ITestee ObjectUnderTest { get; set; }

    [Test]
    void Do()
    {
        var result = ObjectUnderTest.DoSomething();

        ###;
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [TestCase("Assert.AreEqual(value, ObjectUnderTest.TestValue)", "Assert.That(ObjectUnderTest.TestValue, Is.EqualTo(value))")]
        [TestCase("Assert.AreEqual(ObjectUnderTest.TestValue, value)", "Assert.That(ObjectUnderTest.TestValue, Is.EqualTo(value))")]
        [TestCase("Assert.AreNotEqual(value, ObjectUnderTest.TestValue)", "Assert.That(ObjectUnderTest.TestValue, Is.Not.EqualTo(value))")]
        [TestCase("Assert.AreNotEqual(ObjectUnderTest.TestValue, value)", "Assert.That(ObjectUnderTest.TestValue, Is.Not.EqualTo(value))")]
        public void Code_gets_fixed_for_Assert_on_constant_value_defined_in_method_(string originalCode, string fixedCode)
        {
            const string Template = @"
using System;
using NUnit.Framework;

public interface ITestee
{
    string TestValue { get; }
}

[TestFixture]
public class TestMe
{
    private ITestee ObjectUnderTest { get; set; }

    [Test]
    public void Do()
    {
        const string value = ""some value"";

        ###;
    }
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [TestCase("Assert.AreEqual(Value, ObjectUnderTest.TestValue)", "Assert.That(ObjectUnderTest.TestValue, Is.EqualTo(Value))")]
        [TestCase("Assert.AreEqual(ObjectUnderTest.TestValue, Value)", "Assert.That(ObjectUnderTest.TestValue, Is.EqualTo(Value))")]
        [TestCase("Assert.AreNotEqual(Value, ObjectUnderTest.TestValue)", "Assert.That(ObjectUnderTest.TestValue, Is.Not.EqualTo(Value))")]
        [TestCase("Assert.AreNotEqual(ObjectUnderTest.TestValue, Value)", "Assert.That(ObjectUnderTest.TestValue, Is.Not.EqualTo(Value))")]
        public void Code_gets_fixed_for_Assert_on_constant_value_defined_in_class_(string originalCode, string fixedCode)
        {
            const string Template = @"
using System;
using NUnit.Framework;

public interface ITestee
{
    string TestValue { get; }
}

[TestFixture]
public class TestMe
{
    private ITestee ObjectUnderTest { get; set; }

    [Test]
    public void Do()
    {
        ###;
    }

    private const string Value = ""some value"";
}";

            VerifyCSharpFix(Template.Replace("###", originalCode), Template.Replace("###", fixedCode));
        }

        [Test] // TODO Fix me later because currently we do not know how to fix that situation
        public void Code_gets_currently_not_fixed_for_Assert_Throws_if_exception_is_assigned()
        {
            const string Template = @"
using System;
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    void Do()
    {
        var ex = Assert.Throws<Exception>(() => { });
    }
}";

            VerifyCSharpFix(Template, Template);
        }

        protected override string GetDiagnosticId() => MiKo_3105_TestMethodsUseAssertThatAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3105_TestMethodsUseAssertThatAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3105_CodeFixProvider();
    }
}
