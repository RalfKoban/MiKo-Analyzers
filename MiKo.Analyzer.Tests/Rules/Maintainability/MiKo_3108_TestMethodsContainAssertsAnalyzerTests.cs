using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3108_TestMethodsContainAssertsAnalyzerTests : CodeFixVerifier
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
        public void No_issue_is_reported_for_a_test_method_that_uses_an_assertion_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
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
        public void No_issue_is_reported_for_a_test_method_that_invokes_a_private_method_which_uses_an_assertion_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething()
        {
            DoSomethingCore();
        }

        private void DoSomethingCore()
        {
            Assert.That(42, Is.Not.EqualTo(0815));
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_invokes_a_private_static_generic_method_which_uses_an_assertion_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething()
        {
            DoSomethingCore(42);
        }

        private static void DoSomethingCore<T>(T value) where T : struct, IEquatable<T>
        {
            Assert.That(value, Is.Not.EqualTo(0815));
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_body_that_invokes_a_private_method_which_uses_an_assertion_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething() => DoSomethingCore();

        private void DoSomethingCore()
        {
            Assert.That(42, Is.Not.EqualTo(0815));
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_invokes_an_inherited_method_which_uses_an_assertion_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public abstract class TestMeCore
    {
        protected void DoSomethingCore()
        {
            Assert.That(42, Is.Not.EqualTo(0815));
        }
    }

    public class TestMe : TestMeCore
    {
        [" + test + @"]
        public void DoSomething()
        {
            DoSomethingCore();
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_uses_an_fluent_assertion_([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace FluentAssertions
{
    public static class AssertionExtensions
    {
        public static BooleanAssertions Should(this bool actualValue)
        {
            return new BooleanAssertions(actualValue);
        }
    }
}

namespace FluentAssertions.Primitives
{
    public class BooleanAssertions
    {
        public BooleanAssertions(bool? value)
        {
        }

        public void BeFalse()
        {
        }
    }
}

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething()
        {
            bool x = false;
            x.Should().BeFalse();
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_returns_a_value() => No_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [TestCase]
        public bool DoSomething()
        {
            bool x = false;
            return x;
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_invokes_a_Moq_Verify_with_Verifiable() => No_issue_is_reported_for(@"
using NUnit.Framework;
using Moq;
using System;

namespace Bla
{
    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock = new Mock<IDisposable>();

            mock.Setup(_ => _.Dispose()).Verifiable();

            mock.Verify();
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_invokes_a_Moq_Verify_with_Verifiable_and_message() => No_issue_is_reported_for(@"
using NUnit.Framework;
using Moq;
using System;

namespace Bla
{
    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock = new Mock<IDisposable>();

            mock.Setup(_ => _.Dispose()).Verifiable(""test message"");

            mock.Verify();
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_invokes_a_Moq_Verify_call() => No_issue_is_reported_for(@"
using NUnit.Framework;
using Moq;
using System;

namespace Bla
{
    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock = new Mock<IDisposable>();

            mock.Verify(_ => _.Dispose());
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_invokes_a_Moq_VerifyGet_call() => No_issue_is_reported_for(@"
using NUnit.Framework;
using Moq;
using System;

namespace Bla
{
    public interface ITestable
    {
        string Name { get; set; }
    }

    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock = new Mock<ITestable>();

            mock.VerifyGet(_ => _.Name);
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_invokes_a_Moq_VerifySet_call() => No_issue_is_reported_for(@"
using NUnit.Framework;
using Moq;
using System;

namespace Bla
{
    public interface ITestable
    {
        string Name { get; set; }
    }

    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock = new Mock<ITestable>();

            mock.VerifySet(_ => _.Name = ""abc"");
        }
    }
}");

        [Test]
        public void No_issue_is_reported_for_a_test_method_that_invokes_a_Moq_VerifyAll_call() => No_issue_is_reported_for(@"
using NUnit.Framework;
using Moq;
using System;

namespace Bla
{
    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock = new Mock<IDisposable>();

            mock.VerifyAll();
        }
    }
}");

        [TestCase("Received()")]
        [TestCase("Received(42)")]
        [TestCase("ReceivedWithAnyArgs()")]
        [TestCase("DidNotReceive()")]
        [TestCase("DidNotReceiveWithAnyArgs()")]
        public void No_issue_is_reported_for_a_test_method_that_invokes_a_NSubstitute_call(string call) => No_issue_is_reported_for(@"
using NUnit.Framework;
using NSubstitute;
using System;

namespace Bla
{
    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock = Substitute.For<IDisposable>();

            mock." + call + @".Dispose();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_an_empty_test_method_([ValueSource(nameof(Tests))] string test) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething()
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_method_that_does_not_contain_any_assertion_([ValueSource(nameof(Tests))] string test) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething()
        {
             var x = 0;
             var y = x.ToString();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_a_test_method_that_returns_a_Task_but_does_not_assert_anything() => An_issue_is_reported_for(@"
using NUnit.Framework;
using System.Threading.Tasks;

namespace Bla
{
    public class TestMe
    {
        [Test]
        public Task DoSomething()
        {
            return Task.CompletedTask;
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_test_method_body_that_invokes_a_private_method_which_does_not_use_an_assertion_([ValueSource(nameof(Tests))] string test) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public class TestMe
    {
        [" + test + @"]
        public void DoSomething() => DoSomethingCore();

        private void DoSomethingCore()
        {
            // missing assertion
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_test_method_that_invokes_an_inherited_method_which_does_not_use_an_assertion_([ValueSource(nameof(Tests))] string test) => An_issue_is_reported_for(@"
using NUnit.Framework;

namespace Bla
{
    public abstract class TestMeCore
    {
        protected void DoSomethingCore()
        {
            // missing assertion
        }
    }

    public class TestMe : TestMeCore
    {
        [" + test + @"]
        public void DoSomething()
        {
            DoSomethingCore();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_test_method_that_invokes_a_Moq_Verify_but_no_Verifiable() => An_issue_is_reported_for(@"
using NUnit.Framework;
using Moq;
using System;

namespace Bla
{
    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock = new Mock<IDisposable>();

            mock.Verify();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_test_method_that_invokes_a_Moq_Verify_on_one_mock_but_sets_Verifiable_on_another_mock() => An_issue_is_reported_for(@"
using NUnit.Framework;
using Moq;
using System;

namespace Bla
{
    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock1 = new Mock<IDisposable>();
            var mock2 = new Mock<IDisposable>();

            mock2.Setup(_ => _.Dispose()).Verifiable();

            mock1.Verify();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_test_method_that_invokes_a_Moq_Verify_on_one_mock_but_sets_Verifiable_with_message_on_another_mock() => An_issue_is_reported_for(@"
using NUnit.Framework;
using Moq;
using System;

namespace Bla
{
    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock1 = new Mock<IDisposable>();
            var mock2 = new Mock<IDisposable>();

            mock2.Setup(_ => _.Dispose()).Verifiable(""test message"");

            mock1.Verify();
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_a_test_method_that_invokes_no_NSubstitute_Received_or_DidNotReceive_call() => An_issue_is_reported_for(@"
using NUnit.Framework;
using NSubstitute;
using System;

namespace Bla
{
    public class TestMe
    {
        [TestCase]
        public void DoSomething()
        {
            var mock = Substitute.For<IDisposable>();
        }
    }
}");

        protected override string GetDiagnosticId() => MiKo_3108_TestMethodsContainAssertsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3108_TestMethodsContainAssertsAnalyzer();
    }
}