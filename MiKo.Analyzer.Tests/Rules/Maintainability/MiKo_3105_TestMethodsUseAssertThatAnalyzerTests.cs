using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
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
        public void No_issue_is_reported_for_preferred_usage_in_a_non_test_method_inside_a_test_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [" + fixture + @"]
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
        public void An_issue_is_reported_for_a_non_test_method_inside_a_test_([ValueSource(nameof(TestFixtures))] string fixture) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    [" + fixture + @"]
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
             "public void Do(object o) => Assert.IsNull(o);",
             "public void Do(object o) => Assert.That(o, Is.Null);")]

        // Null
        [TestCase(
             "public void Do(object o) => Assert.Null(o);",
             "public void Do(object o) => Assert.That(o, Is.Null);")]
        [TestCase(
             @"public void Do(object o) => Assert.Null(o, ""my message"");",
             @"public void Do(object o) => Assert.That(o, Is.Null, ""my message"");")]

        // NotNull
        [TestCase(
             "public void Do(object o) => Assert.NotNull(o);",
             "public void Do(object o) => Assert.That(o, Is.Not.Null);")]
        [TestCase(
             @"public void Do(object o) => Assert.NotNull(o, ""my message"");",
             @"public void Do(object o) => Assert.That(o, Is.Not.Null, ""my message"");")]

        // IsNotEmpty
        [TestCase(
             "public void Do(object o) => Assert.IsNotEmpty(o);",
             "public void Do(object o) => Assert.That(o, Is.Not.Empty);")]
        [TestCase(
             @"public void Do(object o) => Assert.IsNotEmpty(o, ""my message"");",
             @"public void Do(object o) => Assert.That(o, Is.Not.Empty, ""my message"");")]

        // IsTrue
        [TestCase(
             "public void Do(bool b) => Assert.IsTrue(b);",
             "public void Do(bool b) => Assert.That(b, Is.True);")]
        [TestCase(
             @"public void Do(bool b) => Assert.IsTrue(b, ""my message"");",
             @"public void Do(bool b) => Assert.That(b, Is.True, ""my message"");")]
        [TestCase(
             "public void Do(object a, object b) => Assert.IsTrue(a == b);",
             "public void Do(object a, object b) => Assert.That(a, Is.EqualTo(b));")]
        [TestCase(
             "public void Do(object a, object b) => Assert.IsTrue(a != b);",
             "public void Do(object a, object b) => Assert.That(a, Is.Not.EqualTo(b));")]
        [TestCase(
             "public void Do(object o) => Assert.IsTrue(o == null);",
             "public void Do(object o) => Assert.That(o, Is.Null);")]
        [TestCase(
             "public void Do(object o) => Assert.IsTrue(o != null);",
             "public void Do(object o) => Assert.That(o, Is.Not.Null);")]
        [TestCase(
             "public void Do(object o) => Assert.IsTrue(null == o);",
             "public void Do(object o) => Assert.That(o, Is.Null);")]
        [TestCase(
             "public void Do(object o) => Assert.IsTrue(null != o);",
             "public void Do(object o) => Assert.That(o, Is.Not.Null);")]
        [TestCase(
             "public void Do(int i) => Assert.IsTrue(i == 1);",
             "public void Do(int i) => Assert.That(i, Is.EqualTo(1));")]
        [TestCase(
             "public void Do(int i) => Assert.IsTrue(i < 5);",
             "public void Do(int i) => Assert.That(i, Is.LessThan(5));")]
        [TestCase(
             "public void Do(int i) => Assert.IsTrue(i <= 5);",
             "public void Do(int i) => Assert.That(i, Is.LessThanOrEqualTo(5));")]
        [TestCase(
             "public void Do(int i) => Assert.IsTrue(i > 5);",
             "public void Do(int i) => Assert.That(i, Is.GreaterThan(5));")]
        [TestCase(
             "public void Do(int i) => Assert.IsTrue(i >= 5);",
             "public void Do(int i) => Assert.That(i, Is.GreaterThanOrEqualTo(5));")]

        // True
        [TestCase(
             "public void Do(bool b) => Assert.True(b);",
             "public void Do(bool b) => Assert.That(b, Is.True);")]

        // IsFalse
        [TestCase(
             "public void Do(bool b) => Assert.IsFalse(b);",
             "public void Do(bool b) => Assert.That(b, Is.False);")]
        [TestCase(
             @"public void Do(bool b) => Assert.IsFalse(b, ""my message"");",
             @"public void Do(bool b) => Assert.That(b, Is.False, ""my message"");")]
        [TestCase(
             "public void Do(object a, object b) => Assert.IsFalse(a == b);",
             "public void Do(object a, object b) => Assert.That(a, Is.Not.EqualTo(b));")]
        [TestCase(
             "public void Do(object a, object b) => Assert.IsFalse(a != b);",
             "public void Do(object a, object b) => Assert.That(a, Is.EqualTo(b));")]
        [TestCase(
             "public void Do(object o) => Assert.IsFalse(o == null);",
             "public void Do(object o) => Assert.That(o, Is.Not.Null);")]
        [TestCase(
             "public void Do(object o) => Assert.IsFalse(o != null);",
             "public void Do(object o) => Assert.That(o, Is.Null);")]
        [TestCase(
             "public void Do(object o) => Assert.IsFalse(null == o);",
             "public void Do(object o) => Assert.That(o, Is.Not.Null);")]
        [TestCase(
             "public void Do(object o) => Assert.IsFalse(null != o);",
             "public void Do(object o) => Assert.That(o, Is.Null);")]
        [TestCase(
             "public void Do(int i) => Assert.IsFalse(i == 1);",
             "public void Do(int i) => Assert.That(i, Is.Not.EqualTo(1));")]
        [TestCase(
             "public void Do(int i) => Assert.IsFalse(i < 5);",
             "public void Do(int i) => Assert.That(i, Is.GreaterThanOrEqualTo(5));")]
        [TestCase(
             "public void Do(int i) => Assert.IsFalse(i <= 5);",
             "public void Do(int i) => Assert.That(i, Is.GreaterThan(5));")]
        [TestCase(
             "public void Do(int i) => Assert.IsFalse(i > 5);",
             "public void Do(int i) => Assert.That(i, Is.LessThanOrEqualTo(5));")]
        [TestCase(
             "public void Do(int i) => Assert.IsFalse(i >= 5);",
             "public void Do(int i) => Assert.That(i, Is.LessThan(5));")]
        [TestCase(
             @"public void Do() => Assert.IsFalse(File.Exists(""a.txt"");",
             @"public void Do() => Assert.That(File.Exists(""a.txt""), Is.False);",
             "using System.IO;")]

        // False
        [TestCase(
             "public void Do(bool b) => Assert.False(b);",
             "public void Do(bool b) => Assert.That(b, Is.False);")]

        // AreEqual
        [TestCase(
             "public void Do() => Assert.AreEqual(42, 11);",
             "public void Do() => Assert.That(11, Is.EqualTo(42));")]
        [TestCase(
             @"public void Do() => Assert.AreEqual(42, 11, ""my message"");",
             @"public void Do() => Assert.That(11, Is.EqualTo(42), ""my message"");")]
        [TestCase(
             "public void Do(bool b) => Assert.AreEqual(true, b);",
             "public void Do(bool b) => Assert.That(b, Is.True);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreEqual(b, true);",
             "public void Do(bool b) => Assert.That(b, Is.True);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreEqual(false, b);",
             "public void Do(bool b) => Assert.That(b, Is.False);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreEqual(b, false);",
             "public void Do(bool b) => Assert.That(b, Is.False);")]
        [TestCase(
             "public void Do(int i) => Assert.AreEqual(i, 42);",
             "public void Do(int i) => Assert.That(i, Is.EqualTo(42));")]
        [TestCase(
             "public void Do(int i) => Assert.AreEqual(42, i);",
             "public void Do(int i) => Assert.That(i, Is.EqualTo(42));")]
        [TestCase(
             @"public void Do(string s) => Assert.AreEqual(s, ""abc"");",
             @"public void Do(string s) => Assert.That(s, Is.EqualTo(""abc""));")]
        [TestCase(
             @"public void Do(string s) => Assert.AreEqual(""abc"", s);",
             @"public void Do(string s) => Assert.That(s, Is.EqualTo(""abc""));")]
        [TestCase(
             "public void Do(object o) => Assert.AreEqual(null, o);",
             "public void Do(object o) => Assert.That(o, Is.Null);")]
        [TestCase(
             "public void Do(object o) => Assert.AreEqual(o, null);",
             "public void Do(object o) => Assert.That(o, Is.Null);")]
        [TestCase(
             "public void Do(double d) => Assert.AreEqual(8.5d, d, double.Epsilon);",
             "public void Do(double d) => Assert.That(d, Is.EqualTo(8.5d).Within(double.Epsilon));")]
        [TestCase(
             @"public void Do(double d) => Assert.AreEqual(8.5d, d, double.Epsilon, ""some message"");",
             @"public void Do(double d) => Assert.That(d, Is.EqualTo(8.5d).Within(double.Epsilon), ""some message"");")]
        [TestCase(
             @"public void Do(double d) => Assert.AreEqual(8.5d, d, 0.1d, ""some message"");",
             @"public void Do(double d) => Assert.That(d, Is.EqualTo(8.5d).Within(0.1d), ""some message"");")]
        [TestCase(
             @"public void Do(double d) => Assert.AreEqual(d, 8.5d, double.Epsilon, ""some message"");",
             @"public void Do(double d) => Assert.That(d, Is.EqualTo(8.5d).Within(double.Epsilon), ""some message"");")]
        [TestCase(
             @"public void Do(double d) => Assert.AreEqual(d, 8.5d, 0.1d, ""some message"");",
             @"public void Do(double d) => Assert.That(d, Is.EqualTo(8.5d).Within(0.1d), ""some message"");")]
        [TestCase(
             "public void Do(GCNotificationStatus status) => Assert.AreEqual(status, GCNotificationStatus.Failed);",
             "public void Do(GCNotificationStatus status) => Assert.That(status, Is.EqualTo(GCNotificationStatus.Failed));")]
        [TestCase(
             "public void Do(GCNotificationStatus status) => Assert.AreEqual(GCNotificationStatus.Failed, status);",
             "public void Do(GCNotificationStatus status) => Assert.That(status, Is.EqualTo(GCNotificationStatus.Failed));")]

        // AreEqual (expected & actual variables)
        [TestCase(
             "public void Do(int actual, int expected) => Assert.AreEqual(expected, actual);",
             "public void Do(int actual, int expected) => Assert.That(actual, Is.EqualTo(expected));")]
        [TestCase(
             "public void Do(int actual, int expected) => Assert.AreEqual(actual, expected);",
             "public void Do(int actual, int expected) => Assert.That(actual, Is.EqualTo(expected));")]
        [TestCase(
             "public void Do(int actualValue, int expectedValue) => Assert.AreEqual(expectedValue, actualValue);",
             "public void Do(int actualValue, int expectedValue) => Assert.That(actualValue, Is.EqualTo(expectedValue));")]
        [TestCase(
             "public void Do(int actualValue, int expectedValue) => Assert.AreEqual(actualValue, expectedValue);",
             "public void Do(int actualValue, int expectedValue) => Assert.That(actualValue, Is.EqualTo(expectedValue));")]
        [TestCase(
             "public void Do(int actual, int other) => Assert.AreEqual(other, actual);",
             "public void Do(int actual, int other) => Assert.That(actual, Is.EqualTo(other));")]
        [TestCase(
             "public void Do(int actual, int other) => Assert.AreEqual(actual, other);",
             "public void Do(int actual, int other) => Assert.That(actual, Is.EqualTo(other));")]
        [TestCase(
             "public void Do(int actualValue, int otherValue) => Assert.AreEqual(otherValue, actualValue);",
             "public void Do(int actualValue, int otherValue) => Assert.That(actualValue, Is.EqualTo(otherValue));")]
        [TestCase(
             "public void Do(int actualValue, int otherValue) => Assert.AreEqual(actualValue, otherValue);",
             "public void Do(int actualValue, int otherValue) => Assert.That(actualValue, Is.EqualTo(otherValue));")]

        // AreNotEqual
        [TestCase(
             "public void Do() => Assert.AreNotEqual(42, 11);",
             "public void Do() => Assert.That(11, Is.Not.EqualTo(42));")]
        [TestCase(
             @"public void Do() => Assert.AreNotEqual(42, 11, ""my message"");",
             @"public void Do() => Assert.That(11, Is.Not.EqualTo(42), ""my message"");")]
        [TestCase(
             "public void Do(bool b) => Assert.AreNotEqual(true, b);",
             "public void Do(bool b) => Assert.That(b, Is.False);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreNotEqual(b, true);",
             "public void Do(bool b) => Assert.That(b, Is.False);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreNotEqual(false, b);",
             "public void Do(bool b) => Assert.That(b, Is.True);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreNotEqual(b, false);",
             "public void Do(bool b) => Assert.That(b, Is.True);")]
        [TestCase(
             "public void Do(object o) => Assert.AreNotEqual(null, o);",
             "public void Do(object o) => Assert.That(o, Is.Not.Null);")]
        [TestCase(
             "public void Do(object o) => Assert.AreNotEqual(o, null);",
             "public void Do(object o) => Assert.That(o, Is.Not.Null);")]
        [TestCase(
             "public void Do(int i) => Assert.AreNotEqual(i, 42);",
             "public void Do(int i) => Assert.That(i, Is.Not.EqualTo(42));")]
        [TestCase(
             "public void Do(int i) => Assert.AreNotEqual(42, i);",
             "public void Do(int i) => Assert.That(i, Is.Not.EqualTo(42));")]

        // AreNotEqual (expected & actual variables)
        [TestCase(
             "public void Do(int actual, int expected) => Assert.AreNotEqual(expected, actual);",
             "public void Do(int actual, int expected) => Assert.That(actual, Is.Not.EqualTo(expected));")]
        [TestCase(
             "public void Do(int actual, int expected) => Assert.AreNotEqual(actual, expected);",
             "public void Do(int actual, int expected) => Assert.That(actual, Is.Not.EqualTo(expected));")]
        [TestCase(
             "public void Do(int actualValue, int expectedValue) => Assert.AreNotEqual(expectedValue, actualValue);",
             "public void Do(int actualValue, int expectedValue) => Assert.That(actualValue, Is.Not.EqualTo(expectedValue));")]
        [TestCase(
             "public void Do(int actualValue, int expectedValue) => Assert.AreNotEqual(actualValue, expectedValue);",
             "public void Do(int actualValue, int expectedValue) => Assert.That(actualValue, Is.Not.EqualTo(expectedValue));")]
        [TestCase(
             "public void Do(int actual, int other) => Assert.AreNotEqual(other, actual);",
             "public void Do(int actual, int other) => Assert.That(actual, Is.Not.EqualTo(other));")]
        [TestCase(
             "public void Do(int actual, int other) => Assert.AreNotEqual(actual, other);",
             "public void Do(int actual, int other) => Assert.That(actual, Is.Not.EqualTo(other));")]
        [TestCase(
             "public void Do(int actualValue, int otherValue) => Assert.AreNotEqual(otherValue, actualValue);",
             "public void Do(int actualValue, int otherValue) => Assert.That(actualValue, Is.Not.EqualTo(otherValue));")]
        [TestCase(
             "public void Do(int actualValue, int otherValue) => Assert.AreNotEqual(actualValue, otherValue);",
             "public void Do(int actualValue, int otherValue) => Assert.That(actualValue, Is.Not.EqualTo(otherValue));")]

        // Greater, GreaterOrEqual, Less, LessOrEqual
        [TestCase(
             "public void Do() => Assert.Greater(42, 11);",
             "public void Do() => Assert.That(42, Is.GreaterThan(11));")]
        [TestCase(
             @"public void Do() => Assert.Greater(42, 11, ""my message"");",
             @"public void Do() => Assert.That(42, Is.GreaterThan(11), ""my message"");")]
        [TestCase(
             "public void Do() => Assert.GreaterOrEqual(42, 11);",
             "public void Do() => Assert.That(42, Is.GreaterThanOrEqualTo(11));")]
        [TestCase(
             @"public void Do() => Assert.GreaterOrEqual(42, 11, ""my message"");",
             @"public void Do() => Assert.That(42, Is.GreaterThanOrEqualTo(11), ""my message"");")]
        [TestCase(
             "public void Do() => Assert.Less(42, 11);",
             "public void Do() => Assert.That(42, Is.LessThan(11));")]
        [TestCase(
             @"public void Do() => Assert.Less(42, 11, ""my message"");",
             @"public void Do() => Assert.That(42, Is.LessThan(11), ""my message"");")]
        [TestCase(
             "public void Do() => Assert.LessOrEqual(42, 11);",
             "public void Do() => Assert.That(42, Is.LessThanOrEqualTo(11));")]
        [TestCase(
             @"public void Do() => Assert.LessOrEqual(42, 11, ""my message"");",
             @"public void Do() => Assert.That(42, Is.LessThanOrEqualTo(11), ""my message"");")]

        // AreSame
        [TestCase(
             "public void Do(object o) => Assert.AreSame(null, o);",
             "public void Do(object o) => Assert.That(o, Is.Null);")]
        [TestCase(
             "public void Do(object o) => Assert.AreSame(o, null);",
             "public void Do(object o) => Assert.That(o, Is.Null);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreSame(false, b);",
             "public void Do(bool b) => Assert.That(b, Is.False);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreSame(b, false);",
             "public void Do(bool b) => Assert.That(b, Is.False);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreSame(b, true);",
             "public void Do(bool b) => Assert.That(b, Is.True);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreSame(true, b);",
             "public void Do(bool b) => Assert.That(b, Is.True);")]
        [TestCase(
             "public void Do(int i) => Assert.AreSame(42, i);",
             "public void Do(int i) => Assert.That(i, Is.SameAs(42));")]
        [TestCase(
             "public void Do(int i) => Assert.AreSame(i, 42);",
             "public void Do(int i) => Assert.That(i, Is.SameAs(42));")]

        // AreNotSame
        [TestCase(
             "public void Do(object o) => Assert.AreNotSame(null, o);",
             "public void Do(object o) => Assert.That(o, Is.Not.Null);")]
        [TestCase(
             "public void Do(object o) => Assert.AreNotSame(o, null);",
             "public void Do(object o) => Assert.That(o, Is.Not.Null);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreNotSame(false, b);",
             "public void Do(bool b) => Assert.That(b, Is.True);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreNotSame(b, false);",
             "public void Do(bool b) => Assert.That(b, Is.True);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreNotSame(b, true);",
             "public void Do(bool b) => Assert.That(b, Is.False);")]
        [TestCase(
             "public void Do(bool b) => Assert.AreNotSame(true, b);",
             "public void Do(bool b) => Assert.That(b, Is.False);")]
        [TestCase(
             "public void Do(int i) => Assert.AreNotSame(42, i);",
             "public void Do(int i) => Assert.That(i, Is.Not.SameAs(42));")]
        [TestCase(
             "public void Do(int i) => Assert.AreNotSame(i, 42);",
             "public void Do(int i) => Assert.That(i, Is.Not.SameAs(42));")]

        [TestCase(
             @"public void Do(object o) => Assert.IsNullOrEmpty(o, ""my message"");",
             @"public void Do(object o) => Assert.That(o, Is.Null.Or.Empty, ""my message"");")]
        [TestCase(
             @"public void Do(object o) => Assert.IsNotNull(o, ""my message"");",
             @"public void Do(object o) => Assert.That(o, Is.Not.Null, ""my message"");")]
        [TestCase(
             @"public void Do(string s) => Assert.IsEmpty(s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Is.Empty, ""my message"");")]
        [TestCase(
             @"public void Do(IEnumerable e, IEnumerable a) => CollectionAssert.AreEquivalent(e, a, ""my message"");",
             @"public void Do(IEnumerable e, IEnumerable a) => Assert.That(a, Is.EquivalentTo(e), ""my message"");")]
        [TestCase(
             @"public void Do(IEnumerable e, IEnumerable a) => CollectionAssert.AreNotEquivalent(e, a, ""my message"");",
             @"public void Do(IEnumerable e, IEnumerable a) => Assert.That(a, Is.Not.EquivalentTo(e), ""my message"");")]
        [TestCase(
             @"public void Do(IEnumerable e, object o) => CollectionAssert.DoesNotContain(e, o, ""my message"");",
             @"public void Do(IEnumerable e, object o) => Assert.That(e, Does.Not.Contain(o), ""my message"");")]
        [TestCase(
             @"public void Do(IEnumerable e, object o) => CollectionAssert.Contains(e, o, ""my message"");",
             @"public void Do(IEnumerable e, object o) => Assert.That(e, Does.Contain(o), ""my message"");")]
        [TestCase(
             @"public void Do(string s) => StringAssert.StartsWith(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Does.StartWith(""abc""), ""my message"");")]
        [TestCase(
             @"public void Do(string s) => StringAssert.EndsWith(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Does.EndWith(""abc""), ""my message"");")]
        [TestCase(
             @"public void Do(string s) => StringAssert.DoesNotContain(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Does.Not.Contain(""abc""), ""my message"");")]
        [TestCase(
             @"public void Do(string s) => StringAssert.DoesNotStartWith(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Does.Not.StartWith(""abc""), ""my message"");")]
        [TestCase(
             @"public void Do(string s) => StringAssert.DoesNotEndWith(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Does.Not.EndWith(""abc""), ""my message"");")]
        [TestCase(
             @"public void Do(string s) => StringAssert.Contains(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Does.Contain(""abc""), ""my message"");")]
        [TestCase(
             @"public void Do(string s) => StringAssert.AreEqualIgnoringCase(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Is.EqualTo(""abc"").IgnoreCase, ""my message"");")]
        [TestCase(
             @"public void Do(string s) => StringAssert.AreNotEqualIgnoringCase(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Is.Not.EqualTo(""abc"").IgnoreCase, ""my message"");")]
        [TestCase(
             @"public void Do(string s) => CollectionAssert.IsSubsetOf(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Is.SubsetOf(""abc""), ""my message"");")]
        [TestCase(
             @"public void Do(string s) => CollectionAssert.IsNotSubsetOf(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Is.Not.SubsetOf(""abc""), ""my message"");")]
        [TestCase(
             @"public void Do(string s) => CollectionAssert.IsSupersetOf(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Is.SupersetOf(""abc""), ""my message"");")]
        [TestCase(
             @"public void Do(string s) => CollectionAssert.IsNotSupersetOf(""abc"", s, ""my message"");",
             @"public void Do(string s) => Assert.That(s, Is.Not.SupersetOf(""abc""), ""my message"");")]
        [TestCase(
             @"public void Do(object o) => Assert.IsInstanceOf(typeof(object), o, ""my message"");",
             @"public void Do(object o) => Assert.That(o, Is.InstanceOf<object>(), ""my message"");")]
        [TestCase(
             @"public void Do(object o) => Assert.IsInstanceOf<object>(o, ""my message"");",
             @"public void Do(object o) => Assert.That(o, Is.InstanceOf<object>(), ""my message"");")]
        [TestCase(
             @"public void Do(object o) => Assert.IsNotInstanceOf(typeof(object), o, ""my message"");",
             @"public void Do(object o) => Assert.That(o, Is.Not.InstanceOf<object>(), ""my message"");")]
        [TestCase(
             @"public void Do(object o) => Assert.IsNotInstanceOf<object>(o, ""my message"");",
             @"public void Do(object o) => Assert.That(o, Is.Not.InstanceOf<object>(), ""my message"");")]
        [TestCase(
             @"public void Do(IEnumerable e) => CollectionAssert.IsOrdered(e, ""my message"");",
             @"public void Do(IEnumerable e) => Assert.That(e, Is.Ordered, ""my message"");")]
        [TestCase(
             @"public void Do(IEnumerable e) => CollectionAssert.AllItemsAreNotNull(e, ""my message"");",
             @"public void Do(IEnumerable e) => Assert.That(e, Is.All.Not.Null, ""my message"");")]
        [TestCase(
             @"public void Do(IEnumerable e) => CollectionAssert.AllItemsAreUnique(e, ""my message"");",
             @"public void Do(IEnumerable e) => Assert.That(e, Is.Unique, ""my message"");")]
        [TestCase(
             "public void Do(int i) => Assert.Zero(i);",
             "public void Do(int i) => Assert.That(i, Is.Zero);")]
        [TestCase(
             "public void Do(int i) => Assert.NotZero(i);",
             "public void Do(int i) => Assert.That(i, Is.Not.Zero);")]
        [TestCase(
             "public void Do(double d) => Assert.IsNaN(d);",
             "public void Do(double d) => Assert.That(d, Is.NaN);")]
        [TestCase(
             "public void Do(int i) => Assert.Negative(i);",
             "public void Do(int i) => Assert.That(i, Is.Negative);")]
        [TestCase(
             "public void Do(int i) => Assert.Positive(i);",
             "public void Do(int i) => Assert.That(i, Is.Positive);")]

        // IsAssignableFrom
        [TestCase(
             "public void Do() => Assert.IsAssignableFrom(typeof(object), new object());",
             "public void Do() => Assert.That(new object(), Is.AssignableFrom<object>());")]
        [TestCase(
             @"public void Do() => Assert.IsAssignableFrom(typeof(object), new object(), ""should be assignable"");",
             @"public void Do() => Assert.That(new object(), Is.AssignableFrom<object>(), ""should be assignable"");")]
        [TestCase(
             "public void Do() => Assert.IsAssignableFrom<object>(new object());",
             "public void Do() => Assert.That(new object(), Is.AssignableFrom<object>());")]
        [TestCase(
             "public void Do() => Assert.IsNotAssignableFrom(typeof(object), new object());",
             "public void Do() => Assert.That(new object(), Is.Not.AssignableFrom<object>());")]
        [TestCase(
             @"public void Do() => Assert.IsNotAssignableFrom(typeof(object), new object(), ""should be not assignable"");",
             @"public void Do() => Assert.That(new object(), Is.Not.AssignableFrom<object>(), ""should be not assignable"");")]
        [TestCase(
             "public void Do() => Assert.IsNotAssignableFrom<object>(new object());",
             "public void Do() => Assert.That(new object(), Is.Not.AssignableFrom<object>());")]
        [TestCase(
             "public void Do(Type type) => Assert.IsTrue(type.IsAssignableFrom(typeof(object));",
             "public void Do(Type type) => Assert.That(type, Is.AssignableFrom<object>());")]
        [TestCase(
             "public void Do(Type type) => Assert.IsTrue(type.IsAssignableFrom<object>());",
             "public void Do(Type type) => Assert.That(type, Is.AssignableFrom<object>());")]
        [TestCase(
             "public void Do(Type type) => Assert.IsTrue(type.IsNotAssignableFrom(typeof(object));",
             "public void Do(Type type) => Assert.That(type, Is.Not.AssignableFrom<object>());")]
        [TestCase(
             "public void Do(Type type) => Assert.IsTrue(type.IsNotAssignableFrom<object>());",
             "public void Do(Type type) => Assert.That(type, Is.Not.AssignableFrom<object>());")]
        [TestCase(
             "public void Do(Type type) => Assert.IsFalse(type.IsAssignableFrom(typeof(object));",
             "public void Do(Type type) => Assert.That(type, Is.Not.AssignableFrom<object>());")]
        [TestCase(
             "public void Do(Type type) => Assert.IsFalse(type.IsAssignableFrom<object>());",
             "public void Do(Type type) => Assert.That(type, Is.Not.AssignableFrom<object>());")]
        [TestCase(
             "public void Do(Type type) => Assert.IsFalse(type.IsNotAssignableFrom(typeof(object));",
             "public void Do(Type type) => Assert.That(type, Is.AssignableFrom<object>());")]
        [TestCase(
             "public void Do(Type type) => Assert.IsFalse(type.IsNotAssignableFrom<object>());",
             "public void Do(Type type) => Assert.That(type, Is.AssignableFrom<object>());")]
        [TestCase(
            @"public void AssertIsType<T>(Type type) { var expectedType = typeof(T); Assert.IsTrue(expectedType.IsAssignableFrom(type), ""{0} should implement {1}."", type.FullName, expectedType.FullName); }",
            @"public void AssertIsType<T>(Type type) { var expectedType = typeof(T); Assert.That(expectedType, Is.AssignableFrom(type), $""{type.FullName} should implement {expectedType.FullName}.""); }")]
        [TestCase(
             @"public void AssertIsNotType<T>(Type type) { var expectedType = typeof(T); Assert.IsFalse(expectedType.IsAssignableFrom(type), ""{0} should not implement {1}."", type.FullName, expectedType.FullName); }",
             @"public void AssertIsNotType<T>(Type type) { var expectedType = typeof(T); Assert.That(expectedType, Is.Not.AssignableFrom(type), $""{type.FullName} should not implement {expectedType.FullName}.""); }")]

        // misc
        [TestCase(
             @"public void Do() => StringAssert.IsMatch(""some pattern"", ""actual"");",
             @"public void Do() => Assert.That(""actual"", Does.Match(""some pattern""));",
             "using System.Text.RegularExpressions;")]
        [TestCase(
             @"public void Do() => StringAssert.DoesNotMatch(""some pattern"", ""actual"");",
             @"public void Do() => Assert.That(""actual"", Does.Not.Match(""some pattern""));",
             "using System.Text.RegularExpressions;")]
        [TestCase(
             "public void Do(IEnumerable collection) => CollectionAssert.AllItemsAreInstancesOfType(collection, typeof(string));",
             "public void Do(IEnumerable collection) => Assert.That(collection, Is.All.InstanceOf<string>());")]

        // Exists/DoesNotExist
        [TestCase(
             @"public void Do() => FileAssert.Exists(@""c:\pagefile.sys"");",
             @"public void Do() => Assert.That(@""c:\pagefile.sys"", Does.Exist);")]
        [TestCase(
             @"public void Do() => DirectoryAssert.Exists(@""c:\Windows"");",
             @"public void Do() => Assert.That(@""c:\Windows"", Does.Exist);")]

        [TestCase(
             @"public void Do() => FileAssert.DoesNotExist(@""c:\pagefile.sys"");",
             @"public void Do() => Assert.That(@""c:\pagefile.sys"", Does.Not.Exist);")]
        [TestCase(
             @"public void Do() => DirectoryAssert.DoesNotExist(@""c:\Windows"");",
             @"public void Do() => Assert.That(@""c:\Windows"", Does.Not.Exist);")]

        // Throws / DoesNotThrow
        [TestCase(
             "public void Do() => Assert.Throws<ArgumentNullException>(() => throw new Exception());",
             "public void Do() => Assert.That(() => throw new Exception(), Throws.ArgumentNullException);")]
        [TestCase(
             @"public void Do() => Assert.Throws<ArgumentNullException>(() => throw new Exception(), ""some message"");",
             @"public void Do() => Assert.That(() => throw new Exception(), Throws.ArgumentNullException, ""some message"");")]
        [TestCase(
             "public void Do() => Assert.Throws<ArgumentException>(() => throw new Exception());",
             "public void Do() => Assert.That(() => throw new Exception(), Throws.ArgumentException);")]
        [TestCase(
             @"public void Do() => Assert.Throws<ArgumentException>(() => throw new Exception(), ""some message"");",
             @"public void Do() => Assert.That(() => throw new Exception(), Throws.ArgumentException, ""some message"");")]
        [TestCase(
             "public void Do() => Assert.Throws<InvalidOperationException>(() => throw new Exception());",
             "public void Do() => Assert.That(() => throw new Exception(), Throws.InvalidOperationException);")]
        [TestCase(
             @"public void Do() => Assert.Throws<InvalidOperationException>(() => throw new Exception(), ""some message"");",
             @"public void Do() => Assert.That(() => throw new Exception(), Throws.InvalidOperationException, ""some message"");")]
        [TestCase(
             "public void Do() => Assert.Throws<TargetInvocationException>(() => throw new Exception());",
             "public void Do() => Assert.That(() => throw new Exception(), Throws.TargetInvocationException);")]
        [TestCase(
             @"public void Do() => Assert.Throws<TargetInvocationException>(() => throw new Exception(), ""some message"");",
             @"public void Do() => Assert.That(() => throw new Exception(), Throws.TargetInvocationException, ""some message"");")]
        [TestCase(
             "public void Do() => Assert.Throws<NotSupportedException>(() => throw new Exception());",
             "public void Do() => Assert.That(() => throw new Exception(), Throws.TypeOf<NotSupportedException>());")]
        [TestCase(
             @"public void Do() => Assert.Throws<NotSupportedException>(() => throw new Exception(), ""some message"");",
             @"public void Do() => Assert.That(() => throw new Exception(), Throws.TypeOf<NotSupportedException>(), ""some message"");")]
        [TestCase(
             "public void Do() => Assert.Throws(typeof(ApplicationException), () => throw new Exception());",
             "public void Do() => Assert.That(() => throw new Exception(), Throws.TypeOf<ApplicationException>());")]
        [TestCase(
             @"public void Do() => Assert.Throws(typeof(ApplicationException), () => throw new Exception(), ""some message"");",
             @"public void Do() => Assert.That(() => throw new Exception(), Throws.TypeOf<ApplicationException>(), ""some message"");")]
        [TestCase(
             "public void Do(Type exceptionType) => Assert.Throws(exceptionType, () => throw new Exception());",
             "public void Do(Type exceptionType) => Assert.That(() => throw new Exception(), Throws.TypeOf(exceptionType));")]
        [TestCase(
             "public void Do() => Assert.DoesNotThrow(() => throw new ArgumentException());",
             "public void Do() => Assert.That(() => throw new ArgumentException(), Throws.Nothing);")]
        [TestCase(
             "public void Do() => Assert.DoesNotThrowAsync(async () => await Task.CompletedTask);",
             "public void Do() => Assert.That(async () => await Task.CompletedTask, Throws.Nothing);",
             "using System.Threading.Tasks;")]
        public void Code_gets_fixed_(string originalMethod, string fixedMethod, string additionalNamespace = null)
        {
            var originalCode = @"
using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

" + (additionalNamespace ?? string.Empty) + @"

[TestFixture]
public class TestMe
{
    [Test]
    " + originalMethod + @"
}";

            var fixedCode = @"
using System;
using NUnit.Framework;

" + additionalNamespace + @"

[TestFixture]
public class TestMe
{
    [Test]
    " + fixedMethod + @"
}";

            VerifyCSharpFix(originalCode, fixedCode);
        }

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
    public void Do()
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
    public void Do()
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
    public void Do()
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
    public void Do()
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
    public void Do()
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
    public void Do()
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
