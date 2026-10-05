using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3132_TestAssertsDoNotAssertNullReferenceExceptionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] ValidAsserts =
                                                        [
                                                            "Assert.Catch<InvalidOperationException>(() => 42.ToString());",
                                                            "Assert.That(() => 42.ToString(), Throws.Exception.InstanceOf<InvalidOperationException>());",
                                                            "Assert.That(() => 42.ToString(), Throws.Nothing);",
                                                            "Assert.That(() => 42.ToString(), Throws.TypeOf<ArgumentException>());",
                                                            "Assert.That(() => false, Throws.Exception.With.InnerException.InstanceOf(typeof(NullReferenceException)));",
                                                            "Assert.That(() => false, Throws.Exception.With.InnerException.InstanceOf<NullReferenceException>());",
                                                            "Assert.That(() => false, Throws.Exception.With.InnerException.TypeOf(typeof(NullReferenceException)));",
                                                            "Assert.That(() => false, Throws.Exception.With.InnerException.TypeOf<NullReferenceException>());",
                                                            "Assert.That(() => false, Throws.InnerException.InstanceOf(typeof(NullReferenceException)));",
                                                            "Assert.That(() => false, Throws.InnerException.InstanceOf<NullReferenceException>());",
                                                            "Assert.That(() => false, Throws.InnerException.TypeOf(typeof(NullReferenceException)));",
                                                            "Assert.That(() => false, Throws.InnerException.TypeOf<NullReferenceException>());",
                                                            "Assert.That(new InvalidOperationException(), Is.Not.InstanceOf(typeof(NullReferenceException)));",
                                                            "Assert.That(new InvalidOperationException(), Is.Not.InstanceOf<NullReferenceException>());",
                                                            "Assert.That(new InvalidOperationException(), Is.Not.TypeOf(typeof(NullReferenceException)));",
                                                            "Assert.That(new InvalidOperationException(), Is.Not.TypeOf<NullReferenceException>());",
                                                            "Assert.Throws(typeof(ArgumentException), () => 42.ToString());",
                                                            "Assert.Throws<ArgumentNullException>(() => 42.ToString());",
                                                            "Assert.ThrowsAsync<ArgumentException>(async () => await Task.FromResult(42.ToString()));",
                                                        ];

        private static readonly string[] WrongAsserts =
                                                        [
                                                            "Assert.Catch(typeof(NullReferenceException), () => 42.ToString());",
                                                            "Assert.Catch<NullReferenceException>(() => 42.ToString());",
                                                            "Assert.CatchAsync(typeof(NullReferenceException), async () => await Task.FromResult(42.ToString()));",
                                                            "Assert.CatchAsync<NullReferenceException>(async () => await Task.FromResult(42.ToString()));",
                                                            "Assert.That(() => 42.ToString(), Throws.Exception.InstanceOf(typeof(NullReferenceException)));",
                                                            "Assert.That(() => 42.ToString(), Throws.Exception.InstanceOf<NullReferenceException>());",
                                                            "Assert.That(() => 42.ToString(), Throws.Exception.TypeOf(typeof(NullReferenceException)));",
                                                            "Assert.That(() => 42.ToString(), Throws.Exception.TypeOf<NullReferenceException>());",
                                                            "Assert.That(() => 42.ToString(), Throws.InstanceOf(typeof(NullReferenceException)));",
                                                            "Assert.That(() => 42.ToString(), Throws.InstanceOf<NullReferenceException>());",
                                                            "Assert.That(() => 42.ToString(), Throws.TypeOf(typeof(NullReferenceException)));",
                                                            "Assert.That(() => 42.ToString(), Throws.TypeOf<NullReferenceException>());",
                                                            "Assert.Throws(typeof(NullReferenceException), () => 42.ToString());",
                                                            "Assert.Throws<NullReferenceException>(() => 42.ToString());",
                                                            "Assert.ThrowsAsync(typeof(NullReferenceException), async () => await Task.FromResult(42.ToString()));",
                                                            "Assert.ThrowsAsync<NullReferenceException>(async () => await Task.FromResult(42.ToString()));",
                                                        ];

        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_test_with_other_exception_assert_([ValueSource(nameof(ValidAsserts))] string assertion) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        " + assertion + @"
    }
}
");

        [Test]
        public void No_issue_is_reported_for_non_test_class_with_NullReferenceException_assert_([ValueSource(nameof(WrongAsserts))] string assertion) => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        " + assertion + @"
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_that_asserts_that_a_thrown_NullReferenceException_gets_wrapped_into_another_exception_kind_via_throw_statement() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    void DoStuff(Action action)
    {
        action();
    }
}

public class TestMeTests
{
    TestMe ObjectUnderTest { get; set; }

    [Test]
    public void DoSomething()
    {
        Assert.Throws<NotSupportedException>(() => ObjectUnderTest.DoStuff(() => { throw new NullReferenceException(""Inner exception""); }));
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_that_asserts_that_a_thrown_NullReferenceException_gets_wrapped_into_another_exception_kind() => No_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

public class TestMe
{
    void DoStuff(Action action)
    {
        action();
    }
}

public class TestMeTests
{
    TestMe ObjectUnderTest { get; set; }

    [Test]
    public void DoSomething()
    {
        Assert.Throws<NotSupportedException>(() => ObjectUnderTest.DoStuff(() => throw new NullReferenceException(""Inner exception"")));
    }
}
");

        [Test]
        public void No_issue_is_reported_for_test_that_passes_NullReferenceException_as_argument_and_asserts_nothing_is_thrown() => No_issue_is_reported_for(@"
using System;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    private void Handle(Exception ex)
    {
    }

    [Test]
    public void DoSomething()
    {
        Assert.That(() => Handle(new NullReferenceException()), Throws.Nothing);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_NullReferenceException_assert_using_type_alias() => An_issue_is_reported_for(@"
using System;

using NUnit.Framework;

using NRE = System.NullReferenceException;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        Assert.Throws<NRE>(() => 42.ToString());
    }
}
");

        [Test]
        public void An_issue_is_reported_for_NullReferenceException_assert_in_test_([ValueSource(nameof(WrongAsserts))] string assertion) => An_issue_is_reported_for(@"
using System;
using System.Threading.Tasks;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        " + assertion + @"
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3132_TestAssertsDoNotAssertNullReferenceExceptionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3132_TestAssertsDoNotAssertNullReferenceExceptionAnalyzer();
    }
}