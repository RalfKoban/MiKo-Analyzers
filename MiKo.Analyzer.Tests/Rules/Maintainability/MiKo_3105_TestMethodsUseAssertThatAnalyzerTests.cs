﻿using Microsoft.CodeAnalysis.CodeFixes;
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
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.AreEqual(42, 11); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do() => Assert.That(11, Is.EqualTo(42)); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.IsTrue(b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.True); }")]
        [TestCase(
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.IsFalse(b); }",
             "using System; using NUnit.Framework; [TestFixture] class TestMe { [Test] void Do(bool b) => Assert.That(b, Is.False); }")]
        public void Code_gets_fixed_(string originalCode, string fixedCode) => VerifyCSharpFix(originalCode, fixedCode);

        protected override string GetDiagnosticId() => MiKo_3105_TestMethodsUseAssertThatAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3105_TestMethodsUseAssertThatAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3105_CodeFixProvider();
    }
}
