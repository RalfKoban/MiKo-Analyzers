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

        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNull(o); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNull(o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.NotNull(o); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.NotNull(o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNotEmpty(o); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Empty); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsNotEmpty(o, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Empty, ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.IsTrue(b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.IsTrue(b, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True, ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.IsFalse(b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.IsFalse(b, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False, ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.AreEqual(42, 11); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(11, Is.EqualTo(42)); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.AreEqual(42, 11, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(11, Is.EqualTo(42), ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.AreNotEqual(42, 11); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(11, Is.Not.EqualTo(42)); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.AreNotEqual(42, 11, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(11, Is.Not.EqualTo(42), ""my message""); }")]
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
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => CollectionAssert.IsOrdered(e, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => Assert.That(e, Is.Ordered, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => CollectionAssert.AllItemsAreNotNull(e, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => Assert.That(e, Is.All.Not.Null, ""my message""); }")]
        [TestCase(
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => CollectionAssert.AllItemsAreUnique(e, ""my message""); }",
             @"using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(IEnumerable e) => Assert.That(e, Is.Unique, ""my message""); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.IsTrue(a == b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.That(a, Is.EqualTo(b)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.IsFalse(a == b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.That(a, Is.Not.EqualTo(b)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.IsTrue(a != b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.That(a, Is.Not.EqualTo(b)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.IsFalse(a != b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object a, object b) => Assert.That(a, Is.EqualTo(b)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsTrue(o == null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsFalse(o == null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsTrue(o != null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsFalse(o != null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsTrue(null == o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsFalse(null == o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsTrue(null != o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.IsFalse(null != o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsTrue(i == 1); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.EqualTo(1)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.IsFalse(i == 1); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(int i) => Assert.That(i, Is.Not.EqualTo(1)); }")]
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
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreEqual(null, o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreEqual(o, null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreNotEqual(null, o); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.AreNotEqual(o, null); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(object o) => Assert.That(o, Is.Not.Null); }")]
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

        protected override string GetDiagnosticId() => MiKo_3105_TestMethodsUseAssertThatAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3105_TestMethodsUseAssertThatAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3105_CodeFixProvider();
    }
}
