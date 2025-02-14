﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3106_TestAssertsDoNotUseOperatorsAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AssertionMethods =
                                                            [
                                                                nameof(Assert.That),
                                                                "IsTrue",
                                                                "True",
                                                                "IsFalse",
                                                                "False",
                                                            ];

        private static readonly string[] Operators =
                                                     [
                                                         "==",
                                                         "!=",
                                                         "<",
                                                         "<=",
                                                         ">",
                                                         ">=",
                                                     ];

        private static readonly string[] Methods =
                                                   [
                                                       "All",
                                                       "Any",
                                                       "Contains",
                                                       "Equals",
                                                   ];

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
using NUnit.Framework.Legacy;

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
        public void No_issue_is_reported_for_correct_usage_in_a_non_test_method_inside_a_test_([ValueSource(nameof(TestFixtures))] string fixture) => No_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
        public void No_issue_is_reported_for_correct_usage_in_a_non_test_class() => No_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
        public void An_issue_is_reported_for_an_operator_in_a_test_method_(
                                                                       [ValueSource(nameof(AssertionMethods))] string methodName,
                                                                       [ValueSource(nameof(Tests))] string test,
                                                                       [ValueSource(nameof(Operators))] string @operator)
            => An_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething()
        {
            Assert." + methodName + "(42 " + @operator + @" 0815);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_an_operator_in_a_non_test_method_inside_a_test_(
                                                                                         [ValueSource(nameof(AssertionMethods))] string methodName,
                                                                                         [ValueSource(nameof(TestFixtures))] string fixture,
                                                                                         [ValueSource(nameof(Operators))] string @operator)
        => An_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Bla
{
    [" + fixture + @"]
    public class TestMe
    {
        public void DoSomething()
        {
            Assert." + methodName + "(42 " + @operator + @" 0815);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_an_operator_in_a_non_test_class_([ValueSource(nameof(Operators))] string @operator) => An_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
        public void An_issue_is_reported_for_a_Logical_Not_operator_in_a_non_test_class() => An_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool condition)
        {
            Assert.IsTrue(!condition);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_boolean_pattern_in_a_non_test_class_([Values("is true", "is false")] string pattern) => An_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(bool condition)
        {
            Assert.That(condition " + pattern + @");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_null_pattern_in_a_non_test_class() => An_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o)
        {
            Assert.That(o is null);
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_cast_in_a_non_test_class() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Bla
{
    public class TestMe
    {
        public void DoSomething(object o)
        {
            Assert.That(o is IDisposable);
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_an_operator_in_an_Assert_Multiple_([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
        public void An_issue_is_reported_for_a_boolean_operation_in_a_non_test_class_([ValueSource(nameof(Methods))] string method) => An_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

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

        [Test]
        public void No_issue_is_reported_for_NUnit_Contains_operation_in_a_test_class() => No_issue_is_reported_for(@"
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Bla
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void DoSomething()
        {
            var objectUnderTest = new[] { ""test"", ""test2"", ""test3"", };

            Assert.That(objectUnderTest, Does.Contain(""test"").And.Contains(""test2"").And.Contains(""test3""));
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_XUnit_True_with_assertion_message_using_operator_([ValueSource(nameof(Operators))] string @operator) => No_issue_is_reported_for(@"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public void DoSomething()
        {
            Assert.True(42 " + @operator + @" 0815, ""some assertion message"");
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_XUnit_True_without_assertion_message_using_operator_([ValueSource(nameof(Operators))] string @operator) => An_issue_is_reported_for(@"
using Xunit;

namespace Bla
{
    public class TestMe
    {
        [Fact]
        public void DoSomething()
        {
            Assert.True(42 " + @operator + @" 0815);
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3106_TestAssertsDoNotUseOperatorsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3106_TestAssertsDoNotUseOperatorsAnalyzer();
    }
}